using System.Text;
using Liberty.ApplicationShared.Utils;
using Liberty.GmoPaymentGateway;
using Liberty.GmoPaymentGateway.Models.Requests;
using Liberty.GmoPaymentGateway.Services;
using Liberty.Hangfire;
using Liberty.Hangfire.Models;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Settings;
using Liberty.Reservation.Application.Templates;
using Liberty.Reservation.User.Application.Domains.Services.Interfaces;
using Liberty.Reservation.User.WebAPI.Application.Models.Requests;
using Liberty.Reservation.User.WebAPI.Application.Settings;
using Liberty.Reservation.User.WebAPI.Application.Web.ApiService;
using Liberty.SysIntegrationEvent.Events;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;

public class BookingGmoPaymentCommandHandler(
    ILogger<BookingGmoPaymentCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IServiceProvider serviceProvider
) : CreateCommandHandlerBase<BookingGmoPaymentCommand, string>(unitOfWork, null!)
{
    private readonly IBookingHoldManagementService _bookingHoldManagementService
        = serviceProvider.GetRequiredService<IBookingHoldManagementService>();

    private readonly IBookingReservationService _reservationService
        = serviceProvider.GetRequiredService<IBookingReservationService>();

    private readonly IOrderGmoPaymentService _orderGmoPaymentService
        = serviceProvider.GetRequiredService<IOrderGmoPaymentService>();

    private readonly IBookingCheckAvailableService _bookingCheckAvailableService
        = serviceProvider.GetRequiredService<IBookingCheckAvailableService>();

    private readonly IGmoPaymentGatewayService _gmoPaymentGatewayService
        = serviceProvider.GetRequiredService<IGmoPaymentGatewayService>();

    private readonly IExternalApiService _externalApiService
        = serviceProvider.GetRequiredService<IExternalApiService>();

    private readonly IOrderService _orderService
        = serviceProvider.GetRequiredService<IOrderService>();

    private readonly IIntegrationEventOutboxService _integrationEventOutboxService
        = serviceProvider.GetRequiredService<IIntegrationEventOutboxService>();

    private readonly IOptions<GMOPaymentSetting> _gmoPaymentSetting
        = serviceProvider.GetRequiredService<IOptions<GMOPaymentSetting>>();

    private readonly IOptions<MailTemplateSetting> _mailTemplateSetting
        = serviceProvider.GetRequiredService<IOptions<MailTemplateSetting>>();

    private List<IntegrationEventOutbox> EventOutboxes { get; set; } = [];

    private static readonly List<string> FailedStatuses =
    [
        GmoPaymentResult.PayStart,
        GmoPaymentResult.NotFound,
        GmoPaymentResult.Invalid,
        GmoPaymentResult.Failed,
        GmoPaymentResult.Error
    ];

    private const string ResultCodeParameter = "{resultCode}";

    protected override async Task<string> HandleAsync(
        BookingGmoPaymentCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var transactionGmoPlus = TransactionGmoPlus(payload);
        var gmoPayment = MapRequestToGmoPaymentResultRequest(transactionGmoPlus);
        var orderId = gmoPayment.OrderId ?? string.Empty;
        var resultCode = GmoPaymentResult.Failed;
        var paymentSetting = _gmoPaymentSetting.Value;

        var (existingReservation, allowOnlinePayment) = await _bookingCheckAvailableService.GetReservationByOrderIdAsync(
            orderId,
            cancellationToken
        );

        var isSiteLocation = existingReservation?.BookingData?.IsSiteLocation ?? false;
        var reservationId = existingReservation?.Id ?? 0;

        var baseResponseUrl = GetBaseRedirectUrl(
            isSiteLocation,
            existingReservation,
            paymentSetting,
            reservationId
        );

        if (existingReservation is null)
        {
            return baseResponseUrl.Replace(ResultCodeParameter, GmoPaymentResult.NotFound);
        }

        if (IsInvalidReservationState(existingReservation))
        {
            return baseResponseUrl.Replace(ResultCodeParameter, GmoPaymentResult.Invalid);
        }

        var isPaymentReturnSite = transactionGmoPlus is { AccessID: null, ErrCode: null };
        if (isPaymentReturnSite)
        {
            return baseResponseUrl.Replace(ResultCodeParameter, GmoPaymentResult.Failed);
        }

        var reservation = new ReservationEntity
        {
            Id = existingReservation.Id,
            ConfirmedDateTime = DateTime.UtcNow,
            ReservationState = existingReservation.ReservationState,
            PaymentType = existingReservation.PaymentType,
            UpdateCount = existingReservation.UpdateCount
        };

        var isBookingAvailable = false;

        var isPaymentSuccess = transactionGmoPlus is { Result: GmoPaymentResult.PaySuccess, AccessID: not null };
        var isPaymentFail = FailedStatuses.Contains(transactionGmoPlus.Result ?? GmoPaymentResult.Failed);

        if (isPaymentFail)
        {
            reservation.ReservationState = ReservationStatus.Failed;
        }
        else
        {
            isBookingAvailable = await CheckBookingAvailabilityAsync(
                existingReservation,
                cancellationToken
            );

            if (isPaymentSuccess && isBookingAvailable && allowOnlinePayment)
            {
                var order = await _reservationService.GetOrderByReservationIdAsync(existingReservation.Id, cancellationToken);
                order.AccessID = gmoPayment.AccessId;
                await _orderService.UpdateAsync(order, true, cancellationToken: cancellationToken);

                resultCode = HandleReservationPayment(existingReservation, isSiteLocation, reservation);
            }
            else
            {
                reservation.ReservationState = ReservationStatus.Failed;
                resultCode = await OnlinePaymentRefundAsync(existingReservation.Id, orderId);
            }
        }

        var orderNumberId = existingReservation.OrderReservations?.FirstOrDefault()?.OrderId ?? 0;

        var bookingHoldCheckModel = new BookingHoldCheckModel(
            existingReservation.FacilityId,
            existingReservation.SiteId,
            existingReservation.PlanId,
            existingReservation.RoomGroupId,
            existingReservation.CheckInDate,
            existingReservation.RestNumber,
            existingReservation.RoomNumber,
            existingReservation.UserCode,
            existingReservation.Code ?? string.Empty,
            BookingHoldValues.DefaultHoldTimeInSeconds
        );

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var newOrderGmoPaymentResultRequest = new OrderGmoPaymentResultRequest
            {
                OrderId = orderNumberId,
                GmoPaymentResultRequest = gmoPayment,
                ValidDate = DateTime.UtcNow,
                IsEnabled = true
            };

            _ = await _orderGmoPaymentService.CreateAsync(
                newOrderGmoPaymentResultRequest,
                false,
                cancellationToken
            );

            if (reservation.ReservationState is not ReservationStatus.Temporary)
            {
                _ = await _reservationService.ConfirmedAsync(
                    reservation,
                    false,
                    cancellationToken
                );
            }

            await UnitOfWork.CommitAsync(cancellationToken);

            await _bookingHoldManagementService.ReleaseHoldAsync(
                bookingHoldCheckModel,
                cancellationToken
            );

            if (isPaymentSuccess && isBookingAvailable)
            {
                _ = await SendEmail(existingReservation, cancellationToken);
            }

            if (EventOutboxes is { Count: > 0 })
            {
                await _integrationEventOutboxService.CreateRangeAsync(
                    EventOutboxes,
                    true,
                    cancellationToken
                );
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Create GMO payment failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);

            resultCode = DbErrorResultCode(resultCode, !isBookingAvailable);
        }

        return baseResponseUrl.Replace(ResultCodeParameter, resultCode);
    }

    private static string HandleReservationPayment(
        ReservationEntity existingReservation,
        bool isSiteLocation,
        ReservationEntity reservation
    )
    {
        string? resultCode;
        if (!isSiteLocation)
        {
            reservation.ModifiedDateTime = DateTime.UtcNow;
            reservation.UpdateCount += 1;
        }

        if (existingReservation.ReservationState != ReservationStatus.Modified)
        {
            reservation.ReservationState = ReservationStatus.Reserved;
        }

        reservation.PaymentType = PaymentTypes.OnLinePayment;
        resultCode = GmoPaymentResult.Success;
        return resultCode;
    }

    private async Task<bool> CheckBookingAvailabilityAsync(
        ReservationEntity reservation,
        CancellationToken cancellationToken = default
    )
    {
        var isNightNumber = await _bookingCheckAvailableService.IsNightNumberAsync(
            reservation.PlanId,
            reservation.RoomGroupId,
            reservation.SiteId,
            reservation.CheckInDate,
            reservation.RestNumber,
            cancellationToken
        );

        var isRoomNumber = await _bookingCheckAvailableService.IsRoomNumberAsync(
            reservation.PlanId,
            reservation.RoomGroupId,
            reservation.CheckInDate,
            reservation.RoomNumber,
            reservation.RestNumber,
            0,
            cancellationToken
        );

        return isNightNumber && isRoomNumber;
    }

    private static string GetBaseRedirectUrl(
        bool isSiteLocation,
        ReservationEntity? existingReservation,
        GMOPaymentSetting paymentSetting,
        long reservationId
    )
    {
        string baseResponseUrl;
        if (isSiteLocation)
        {
            var facilityCode = existingReservation?.Facility?.Code;
            var siteCode = existingReservation?.Site?.Code;
            baseResponseUrl = paymentSetting.RedirectSiteUrl?
                    .Replace("{facilityCode}", facilityCode)
                    .Replace("{siteCode}", siteCode)
                    .Replace("{reservationId}", $"{reservationId}")
                ?? string.Empty;
        }
        else
        {
            baseResponseUrl = paymentSetting.RedirectUserUrl?.Replace("{reservationId}", $"{reservationId}") ?? string.Empty;
        }

        return baseResponseUrl;
    }

    private static bool IsInvalidReservationState(
        ReservationEntity existingReservation
    )
    {
        return existingReservation is
            {
                PaymentType: PaymentTypes.OnLinePayment,
                ReservationState: not ReservationStatus.Temporary
            }
            or
            {
                PaymentType: PaymentTypes.OnSidePayment,
                ReservationState: not (ReservationStatus.Reserved or ReservationStatus.Modified)
            };
    }

    private static string DbErrorResultCode(
        string? resultCode,
        bool isBookingUnavailable
    )
    {
        if (isBookingUnavailable)
        {
            return resultCode == GmoPaymentResult.RefundSuccess
                ? GmoPaymentResult.RefundSuccessDbError
                : GmoPaymentResult.RefundFailDbError;
        }

        return GmoPaymentResult.DbError;
    }

    private static TransactionResultLinkPlus TransactionGmoPlus(
        GmoPaymentLinkPlusRequest payload
    )
    {
        var base64String = payload.Result.Trim();
        var parts = base64String.Split('.');

        var base64Part = parts[0];

        var byteArray = Convert.FromBase64String(base64Part);
        var decodedText = Encoding.UTF8.GetString(byteArray);

        var transactionResult = System.Text.Json.JsonSerializer.Deserialize<GmoPaymentLinkPlusResponse>(decodedText)?.Transactionresult
            ?? new TransactionResultLinkPlus();

        return transactionResult;
    }

    private static GmoPaymentResultRequest MapRequestToGmoPaymentResultRequest(
        TransactionResultLinkPlus request
    )
    {
        return new GmoPaymentResultRequest
        {
            Code = EntityUtil.CreateCode(),
            RecordMemo = EntityUtil.CreateRecordMemo(),
            AccessId = request.AccessID,
            AccessPass = request.AccessPass,
            OrderId = request.OrderID,
            Status = request.Result,
            ErrCode = request.ErrCode,
            ErrInfo = request.ErrInfo,
            PayType = request.Paymethod
        };
    }

    private async Task<string?> OnlinePaymentRefundAsync(
        long reservationId,
        string orderId
    )
    {
        logger.LogInformation(
            "{Action} - Booking not available online payment refund {ReservationId} {OderId}",
            nameof(OnlinePaymentRefundAsync),
            reservationId,
            orderId
        );

        var requestSearchTrade = new SearchTradeRequest(orderId);
        var responseSearchTrade = await _gmoPaymentGatewayService.SearchTradeAsync(
            requestSearchTrade
        );

        var amountCancel = Convert.ToDecimal(responseSearchTrade.Amount);
        responseSearchTrade.Amount = amountCancel.ToString("F0");

        var requestCancel = new CancelOrderRequest(
            responseSearchTrade.OrderId!,
            responseSearchTrade.AccessId!,
            responseSearchTrade.AccessPass!,
            responseSearchTrade.Amount!,
            responseSearchTrade.Tax!,
            responseSearchTrade.Method!,
            responseSearchTrade.PayTimes!
        );
        var responseCancel = await _gmoPaymentGatewayService.CancelAsync(requestCancel);

        logger.LogInformation(
            "{Action} - Successfully online payment refund {ReservationId} {OderId} {TranId}",
            nameof(OnlinePaymentRefundAsync),
            reservationId,
            orderId,
            responseCancel.TranId
        );

        return responseCancel.HasError ? GmoPaymentResult.RefundFailed : GmoPaymentResult.RefundSuccess;
    }

    private async Task<bool> SendEmail(
        ReservationEntity existingReservation,
        CancellationToken cancellationToken
    )
    {
        var languageCode = existingReservation.BookingData!.LanguageCode;
        var templateFormatData = await _reservationService.FindMailTemplateAsync(
            cancellationToken
        );

        if (templateFormatData is null)
        {
            const string errorMsg = "Mail template not found";
            logger.LogError(errorMsg);
            return false;
        }

        var templateFormatI04 = languageCode switch
        {
            "en" => templateFormatData.CreateTemplate<Io10004EnTemplate>(IoType.IO10004En),
            _ => templateFormatData.CreateTemplate<Io10004Template>(IoType.IO10004)
        };
        _ = await SendEmailToUser(
            existingReservation,
            templateFormatI04,
            cancellationToken
        );

        _ = await SendEmailToFacility(
            existingReservation,
            templateFormatData.CreateTemplate<Io10003Template>(IoType.IO10003),
            cancellationToken
        );

        return true;
    }

    private async Task<bool> SendEmailToUser(
        ReservationEntity existingReservation,
        Io10004Template? mailTemplate,
        CancellationToken cancellationToken
    )
    {
        var bookingId = existingReservation.Id.ToString();
        var facilityId = existingReservation.FacilityId.ToString();
        var toMail = existingReservation.Reserver?.EMail;
        var applicationName = _mailTemplateSetting.Value.ApplicationName ?? string.Empty;

        if (mailTemplate is null)
        {
            const string errorMsg = $"Mail template {IoType.IO10004} not found";
            logger.LogError(errorMsg);
            return false;
        }

        mailTemplate.URL = _mailTemplateSetting.Value.UserUrl ?? string.Empty;
        mailTemplate.SetData(existingReservation, applicationName);
        var message = new
        {
            bookingId,
            tos = new[] { toMail },
            subject = mailTemplate.Subject,
            body = mailTemplate.Body
        };

        return await RegisterScheduleJobAsync(
            bookingId,
            new RegisterScheduleJobDelayRequest(
                nameof(BookingSendMailEvent),
                $"{nameof(BookingSendMailEvent)}_Facility_{facilityId}_Reservation_{bookingId}",
                JsonConvert.SerializeObject(message),
                TimeSpan.FromSeconds(1)
            ),
            cancellationToken
        );
    }

    private async Task<bool> SendEmailToFacility(
        ReservationEntity existingReservation,
        Io10003Template? mailTemplate,
        CancellationToken cancellationToken
    )
    {
        var bookingId = existingReservation.Id.ToString();
        var facilityId = existingReservation.FacilityId.ToString();
        var toMail = existingReservation.Facility?.Meta?.SystemEMail;
        var toFax = existingReservation.Facility?.Fax;
        var isSendFax = !string.IsNullOrWhiteSpace(toFax) && existingReservation.Facility?.UseFax is true;
        var applicationName = _mailTemplateSetting.Value.ApplicationName ?? string.Empty;

        if (mailTemplate is null)
        {
            const string errorMsg = $"Mail template {IoType.IO10003} not found";
            logger.LogError(errorMsg);
            return false;
        }

        mailTemplate.URL = _mailTemplateSetting.Value.ManagerUrl ?? string.Empty;
        mailTemplate.SetData(existingReservation, applicationName);
        var message = new
        {
            bookingId,
            tos = new[] { toMail },
            subject = mailTemplate.Subject,
            body = mailTemplate.Body,
            isSendFax,
            faxNumber = toFax
        };

        return await RegisterScheduleJobAsync(
            bookingId,
            new RegisterScheduleJobDelayRequest(
                nameof(BookingSendMailEvent),
                $"{nameof(BookingSendMailEvent)}_Facility_{facilityId}_Reservation_{bookingId}",
                JsonConvert.SerializeObject(message),
                TimeSpan.FromSeconds(1)
            ),
            cancellationToken
        );
    }

    private async Task<bool> RegisterScheduleJobAsync(
        string bookingId,
        RegisterScheduleJobDelayRequest request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            logger.LogInformation(
                "{Action} Send Batch Job {Reservation} start",
                nameof(BookingGmoPaymentCommandHandler),
                bookingId
            );

            var (_, context) = await _externalApiService.PostAsync(
                ExternalService.BatchSchedulerService,
                RegisterJobEndpoint.RegisterScheduleJobDelayEndpoint,
                JsonConvert.SerializeObject(request),
                cancellationToken
            );

            if (!string.IsNullOrEmpty(context))
            {
                logger.LogInformation(
                    "{Action} Send Batch Job {Reservation} successfully",
                    nameof(BookingGmoPaymentCommandHandler),
                    bookingId
                );
                return true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "{Action} Send Batch Job {Reservation} error {Message}",
                nameof(BookingGmoPaymentCommandHandler),
                bookingId,
                ex.Message
            );

            IntegrationEventOutbox eventOutbox = new()
            {
                ServiceName = DefaultValues.ServiceNameOfUser,
                EventName = request.EventName,
                JobName = request.JobName,
                JsonData = request.JsonData
            };
            EventOutboxes.Add(eventOutbox);
        }

        return false;
    }
}
