using Liberty.GmoPaymentGateway.Exceptions;
using Liberty.GmoPaymentGateway.Models.Requests;
using Liberty.GmoPaymentGateway.Services;
using Liberty.Hangfire;
using Liberty.Hangfire.Models;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Settings;
using Liberty.Reservation.Application.Templates;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Events.Booking;
using Liberty.Reservation.Manager.WebAPI.Application.Web.ApiService;
using Liberty.SysIntegrationEvent.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BookingReservation;

public class BookingCancellationCommandHandler(
    ILogger<BookingCancellationCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IServiceProvider serviceProvider
) : UpdateCommandWithAuditEventHandlerBase<BookingCancellationCommand, long>(unitOfWork, mapper, mediator)
{
    private IBookingCheckAvailableService BookingCheckAvailableService
        => serviceProvider.GetRequiredService<IBookingCheckAvailableService>();

    private IBookingReservationService BookingReservationService
        => serviceProvider.GetRequiredService<IBookingReservationService>();

    private IGmoPaymentGatewayService GmoPaymentGatewayService
        => serviceProvider.GetRequiredService<IGmoPaymentGatewayService>();

    private IMailTemplateService MailTemplateService
        => serviceProvider.GetRequiredService<IMailTemplateService>();

    private IExternalApiService ExternalApiService
        => serviceProvider.GetRequiredService<IExternalApiService>();

    private IBookingSecureUrlService BookingSecureUrlService
        => serviceProvider.GetRequiredService<IBookingSecureUrlService>();

    private IIntegrationEventOutboxService IntegrationEventOutboxService
        => serviceProvider.GetRequiredService<IIntegrationEventOutboxService>();

    private IOptions<MailTemplateSetting> MailTemplateSetting
        => serviceProvider.GetRequiredService<IOptions<MailTemplateSetting>>();

    private IBookingSystemConfigService BookingSystemConfigService
        => serviceProvider.GetRequiredService<IBookingSystemConfigService>();

    private IBookingManagerModificationCheckerService BookingModificationCheckerService
        => serviceProvider.GetRequiredService<IBookingManagerModificationCheckerService>();

    private List<IntegrationEventOutbox> EventOutboxes { get; set; } = [];

    protected override async Task<long> HandleAsync(
        BookingCancellationCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var facilityId = securityContextAccessor.FacilityKey;

        var existingReservation = await BookingCheckAvailableService.GetReservationByFacilityAsync(
            payload.Id ?? 0,
            facilityId,
            cancellationToken
        );

        var cancellationPrice = existingReservation.CancellationPrice;

        EnsureValidReservation(existingReservation);

        existingReservation.CancellationFeeType = GetCancellationFeeType(payload.CancellationFee);

        EnsureManagerCanModify(existingReservation);
        await ValidateGlobalOnlinePayment(existingReservation, cancellationToken);

        var languageCode = existingReservation.BookingData?.LanguageCode ?? securityContextAccessor.GetLanguageCode();

        var timeZone = BookingReservationService.GetFacilityTimeZoneById(facilityId);

        var cancelledDateTime = DateTime.UtcNow.Add(timeZone);

        var cancellationFee = NormalizeCancellationFee(
            payload.CancellationFee,
            existingReservation.BookingData!.AllTotalPrice,
            existingReservation.CancellationStatus
        );

        var reservation = await BookingReservationService.CancelAsync(
            existingReservation,
            cancellationFee,
            0,
            true,
            cancellationToken
        );
        var refundNeeded = ShouldRefund(existingReservation);

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            await ProcessCancellationStatusAsync(refundNeeded, existingReservation, cancellationFee, reservation);

            reservation.ReservationState = ReservationStatus.ManagerCanceled;

            await UnitOfWork.CommitAsync(cancellationToken);

            AuditEventData = new BookingCanceledEvent
            {
                Id = reservation.Id,
                AggregateCode = reservation.Code ?? string.Empty,
                Request = request,
                UserCode = securityContextAccessor.ApplicationUserKey,
                FacilityId = reservation.FacilityId,
                SiteId = reservation.SiteId,
                OldId = null
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Online payment refund reservation cancel failed: {Message}", ex.Message);
            await HandlerRefundErrorAsync(
                reservation,
                cancelledDateTime,
                cancellationPrice,
                cancellationToken
            );

            await SendCancellationEmailAndCreateOutboxEvents(existingReservation, languageCode, cancellationToken);

            CreatePaymentCancelException(existingReservation);
        }

        if (!existingReservation.IsReserved)
        {
            return reservation.Id;
        }

        SetCancelInfoToReservation(existingReservation, reservation);

        _ = await SendEmails(
            existingReservation,
            languageCode,
            cancellationToken
        );

        if (EventOutboxes is { Count: > 0 })
        {
            await IntegrationEventOutboxService.CreateRangeAsync(
                EventOutboxes,
                true,
                cancellationToken
            );
        }

        return reservation.Id;
    }

    private async Task SendCancellationEmailAndCreateOutboxEvents(
        ReservationEntity existingReservation,
        string languageCode,
        CancellationToken cancellationToken
    )
    {
        if (existingReservation.IsReserved)
        {
            try
            {
                _ = await SendEmails(existingReservation, languageCode, cancellationToken);

                if (EventOutboxes is { Count: > 0 })
                {
                    await IntegrationEventOutboxService.CreateRangeAsync(
                        EventOutboxes,
                        true,
                        cancellationToken
                    );
                }
            }
            catch (Exception mailEx)
            {
                logger.LogError(mailEx, "Failed to send cancellation email or create outbox event");
            }
        }
    }

    private async Task ProcessCancellationStatusAsync(
        bool refundNeeded,
        ReservationEntity existingReservation,
        decimal cancellationFee,
        ReservationEntity reservation
    )
    {
        if (refundNeeded)
        {
            _ = await OnlinePaymentRefundAsync(existingReservation.Id, cancellationFee);

            reservation.CancellationStatus = cancellationFee != 0
                ? CancellationStatus.CancelledRefunded
                : CancellationStatus.CancelledDepositRefunded;
        }
        else
        {
            var isFinalized = existingReservation.ReservationState is ReservationStatus.UserCanceled
                || existingReservation.ReservationState is ReservationStatus.GuestCanceled
                || existingReservation.CancellationStatus is CancellationStatus.CancelledLocalPaymentRefunded;

            reservation.CancellationStatus = isFinalized
                ? CancellationStatus.CancelledLocalFinalized
                : CancellationStatus.CancelledLocalPaymentRefunded;
        }
    }

    private static void SetCancelInfoToReservation(
        ReservationEntity existingReservation,
        ReservationEntity request
    )
    {
        existingReservation.CancellationPrice = request.CancellationPrice;
        existingReservation.CancelRateFee = request.CancelRateFee;
        existingReservation.CancelledDateTime = request.CancelledDateTime;
    }

    private static void EnsureValidReservation(
        ReservationEntity existing
    )
    {
        if (!IsValidReservation(existing))
        {
            throw new ReservationInvalidException("Reservation is not in a valid state for this operation.");
        }
    }

    private static CancellationFeeType GetCancellationFeeType(
        decimal? feeFromPayload
    )
    {
        return feeFromPayload is null ? CancellationFeeType.SystemDefined : CancellationFeeType.UserInput;
    }

    private static decimal NormalizeCancellationFee(
        decimal? feeFromPayload,
        decimal totalPrice,
        CancellationStatus? status
    )
    {
        var fee = feeFromPayload ?? 0m;

        if (fee < 0 || fee > totalPrice)
        {
            throw new CancellationFeeInvalidException();
        }

        return status is CancellationStatus.CancelledRefunded ? 0m : fee;
    }

    private void EnsureManagerCanModify(
        ReservationEntity existing
    )
    {
        var canModify = BookingModificationCheckerService.CanCancelModify(
            existing.CheckOutDate,
            existing.IsNoShow,
            existing.ReservationState,
            existing.Facility?.TimeZone ?? DefaultValues.DefaultTimeZoneOffset
        );

        if (!canModify)
        {
            throw new ReservationChangeNotAllowedException();
        }
    }

    private static bool ShouldRefund(
        ReservationEntity existing
    )
    {
        return existing.IsOnlinePayment
            && existing.CancellationStatus != CancellationStatus.CancelledDepositRefunded
            && (existing.CancellationStatus != CancellationStatus.CancelledRefunded || existing.CancellationPrice != 0);
    }

    private static void CreatePaymentCancelException(
        ReservationEntity existingReservation
    )
    {
        if (existingReservation.CancellationStatus is null && existingReservation.IsReserved)
        {
            throw new BookingCancelPaymentException();
        }

        if (existingReservation.ReservationState == ReservationStatus.ManagerCanceled)
        {
            throw existingReservation.CancellationStatus switch
            {
                CancellationStatus.CancelledPaymentIssue => new PaymentAmountChangeException(),
                CancellationStatus.CancelledRefunded => new CancelCancellationFeeException(),
                _ => new PaymentCancelException("Refund payment failed due to payment gateway error.")
            };
        }

        throw new PaymentCancelException("Refund payment failed due to payment gateway error.");
    }

    private static bool IsValidReservation(
        ReservationEntity reservation
    )
    {
        if (reservation.PaymentType == PaymentTypes.OnLinePayment)
        {
            return
                reservation is { IsReserved: true, CancellationStatus: null }
                || (
                    reservation is
                    {
                        ReservationState: ReservationStatus.ManagerCanceled,
                        CancellationStatus: CancellationStatus.CancelledRefunded
                    }
                    && reservation.CancellationPrice != 0
                )
                || reservation.CancellationStatus == CancellationStatus.CancelledPaymentIssue;
        }

        return
            reservation is
                {
                    IsReserved: true, CancellationStatus: null
                }
                or
                {
                    ReservationState: ReservationStatus.UserCanceled
                }
                or
                {
                    ReservationState: ReservationStatus.GuestCanceled
                }
                or
                {
                    ReservationState: ReservationStatus.ManagerCanceled,
                    CancellationStatus: CancellationStatus.CancelledLocalPaymentRefunded
                };
    }

    private async Task ValidateGlobalOnlinePayment(
        ReservationEntity existingReservation,
        CancellationToken cancellationToken
    )
    {
        var systemConfig = await BookingSystemConfigService.GetSystemConfigAsync(cancellationToken)
            ?? throw new BookingSystemConfigNotfoundException();
        if (systemConfig.CanOnlinePayment is false or null && existingReservation.IsOnlinePayment)
        {
            throw new GlobalCanOnlinePaymentNotAllowException();
        }
    }

    private async Task<string?> OnlinePaymentRefundAsync(
        long reservationId,
        decimal cancellationPrice = 0
    )
    {
        var orderId = await BookingReservationService.FindOderIdOfOnlinePaymentAsync(reservationId);

        logger.LogInformation(
            "{Action} - Start online payment refund {ReservationId} {OderId} {CancellationPrice}",
            nameof(OnlinePaymentRefundAsync),
            reservationId,
            orderId,
            cancellationPrice
        );

        var requestSearchTrade = new SearchTradeRequest(orderId);
        var responseSearchTrade = await GmoPaymentGatewayService.SearchTradeAsync(
            requestSearchTrade
        );

        var amountCancel = Convert.ToDecimal(responseSearchTrade.Amount);
        responseSearchTrade.Amount = cancellationPrice > 0
            ? cancellationPrice.ToString("F0")
            : amountCancel.ToString("F0");

        var requestCancel = new CancelOrderRequest(
            responseSearchTrade.OrderId!,
            responseSearchTrade.AccessId!,
            responseSearchTrade.AccessPass!,
            responseSearchTrade.Amount!,
            responseSearchTrade.Tax!,
            responseSearchTrade.Method!,
            responseSearchTrade.PayTimes!
        );

        if (cancellationPrice > 0)
        {
            requestCancel.IsChangeAmount = true;
        }

        var responseCancel = await GmoPaymentGatewayService.CancelAsync(requestCancel);

        logger.LogInformation(
            "{Action} - Successfully online payment refund: Reservation id: {ReservationId}, Order id, {OderId}, Tran id: {TranId}",
            nameof(OnlinePaymentRefundAsync),
            reservationId,
            orderId,
            responseCancel.TranId
        );

        return responseCancel.TranId;
    }

    private async Task HandlerRefundErrorAsync(
        ReservationEntity existingReservation,
        DateTime cancelledDateTime,
        decimal? cancellationPrice,
        CancellationToken cancellationToken
    )
    {
        existingReservation.ReservationState = ReservationStatus.ManagerCanceled;
        existingReservation.CancelledDateTime = cancelledDateTime;
        existingReservation.CancellationPrice = cancellationPrice;
        existingReservation.CancellationStatus ??= CancellationStatus.CancelledPaymentIssue;

        await UnitOfWork.CommitAsync(cancellationToken);
    }

    private async Task<bool> SendEmails(
        ReservationEntity existingReservation,
        string? languageCode,
        CancellationToken cancellationToken
    )
    {
        var templateFormatData = await MailTemplateService.FindMailTemplateAsync(
            cancellationToken
        );
        if (templateFormatData is null)
        {
            const string errorMsg = "Mail template not found";
            logger.LogError(errorMsg);
            return false;
        }

        var templateFormatI06 = languageCode switch
        {
            "en" => templateFormatData.CreateTemplate<Io10006EnTemplate>(IoType.IO10006En),
            _ => templateFormatData.CreateTemplate<Io10006Template>(IoType.IO10006)
        };
        var templateFormatI010011 = languageCode switch
        {
            "en" => templateFormatData.CreateTemplate<Io10011EnTemplate>(IoType.IO10011En),
            _ => templateFormatData.CreateTemplate<Io10011Template>(IoType.IO10011)
        };
        if (!string.IsNullOrWhiteSpace(existingReservation.UserCode))
        {
            _ = await SendEmailToUser(
                existingReservation,
                templateFormatI06,
                cancellationToken
            );
        }
        else
        {
            _ = await SendEmailToGuest(
                existingReservation,
                templateFormatI010011,
                cancellationToken
            );
        }

        _ = await SendEmailToFacility(
            existingReservation,
            templateFormatData.CreateTemplate<Io10005Template>(IoType.IO10005),
            cancellationToken
        );

        return true;
    }

    private async Task<bool> SendEmailToGuest(
        ReservationEntity existingReservation,
        Io10011Template? mailTemplate,
        CancellationToken cancellationToken
    )
    {
        var bookingId = existingReservation.Id.ToString();
        var facilityId = existingReservation.FacilityId.ToString();
        var toMail = existingReservation.Reserver?.EMail;
        var applicationName = MailTemplateSetting.Value.ApplicationName ?? string.Empty;

        if (mailTemplate is null)
        {
            const string errorMsg = $"Mail template {IoType.IO10011} not found";
            logger.LogError(errorMsg);
            return false;
        }

        var validMinutes = (int)(AppDate.GetDateTime(existingReservation.CheckInDate, existingReservation.CheckInTime) - DateTime.UtcNow)
            .TotalMinutes;
        var secureRequest = new BookingSecureUrlRequest(
            bookingId,
            validMinutes
        );
        var guestCode = BookingSecureUrlService.EncryptDataWithHmacSha256(secureRequest);
        mailTemplate.URL = MailTemplateSetting.Value.GuestUrl?.Replace("{guestCode}", guestCode);
        mailTemplate.SetData(
            existingReservation,
            applicationName,
            ReservationOperationTypes.Facility
        );

        var message = new
        {
            bookingId,
            tos = new[] { toMail },
            subject = mailTemplate.Subject,
            body = mailTemplate.Body,
            fromDisplayName = mailTemplate.FromDisplayName,
            triggerSource = MailActorTypes.Manager,
            eventType = string.Empty
        };

        var jsonData = JsonConvert.SerializeObject(message);

        return await RegisterScheduleJobAsync(
            bookingId,
            new RegisterScheduleJobDelayRequest(
                nameof(BookingSendMailEvent),
                $"{nameof(BookingSendMailEvent)}_Facility_{facilityId}_Reservation_{bookingId}",
                jsonData,
                TimeSpan.FromSeconds(1)
            ),
            cancellationToken
        );
    }

    private async Task<bool> SendEmailToUser(
        ReservationEntity existingReservation,
        Io10006Template? mailTemplate,
        CancellationToken cancellationToken
    )
    {
        var bookingId = existingReservation.Id.ToString();
        var facilityId = existingReservation.FacilityId.ToString();
        var toMail = existingReservation.Reserver?.EMail;
        var applicationName = MailTemplateSetting.Value.ApplicationName ?? string.Empty;

        if (mailTemplate is null)
        {
            const string errorMsg = $"Mail template {IoType.IO10006} not found";
            logger.LogError(errorMsg);
            return false;
        }

        mailTemplate.URL = MailTemplateSetting.Value.UserUrl;
        mailTemplate.SetData(
            existingReservation,
            applicationName,
            ReservationOperationTypes.Facility
        );

        var message = new
        {
            bookingId,
            tos = new[] { toMail },
            subject = mailTemplate.Subject,
            body = mailTemplate.Body,
            fromDisplayName = mailTemplate.FromDisplayName,
            triggerSource = MailActorTypes.Manager,
            eventType = string.Empty
        };

        var jsonData = JsonConvert.SerializeObject(message);

        return await RegisterScheduleJobAsync(
            bookingId,
            new RegisterScheduleJobDelayRequest(
                nameof(BookingSendMailEvent),
                $"{nameof(BookingSendMailEvent)}_Facility_{facilityId}_Reservation_{bookingId}",
                jsonData,
                TimeSpan.FromSeconds(1)
            ),
            cancellationToken
        );
    }

    private async Task<bool> SendEmailToFacility(
        ReservationEntity existingReservation,
        Io10005Template? mailTemplate,
        CancellationToken cancellationToken
    )
    {
        var bookingId = existingReservation.Id.ToString();
        var facilityId = existingReservation.FacilityId.ToString();
        var toMail = existingReservation.Facility?.Meta?.SystemEMail;
        var toFax = existingReservation.Facility?.Fax;
        var isSendFax = !string.IsNullOrWhiteSpace(toFax) && existingReservation.Facility?.UseFax is true;
        var applicationName = MailTemplateSetting.Value.ApplicationName ?? string.Empty;

        if (mailTemplate is null)
        {
            const string errorMsg = $"Mail template {IoType.IO10005} not found";
            logger.LogError(errorMsg);
            return false;
        }

        var facilityCode = securityContextAccessor.GetFacilityCode();
        mailTemplate.URL = MailTemplateSetting.Value.ManagerUrl?.Replace("{facilityCode}", facilityCode);
        mailTemplate.SetData(
            existingReservation,
            applicationName,
            ReservationOperationTypes.Facility
        );
        var message = new
        {
            bookingId,
            tos = new[] { toMail },
            subject = mailTemplate.Subject,
            body = mailTemplate.Body,
            isSendFax,
            faxNumber = toFax,
            fromDisplayName = mailTemplate.FromDisplayName,
            triggerSource = MailActorTypes.Manager,
            eventType = string.Empty
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
                nameof(BookingCancellationCommandHandler),
                bookingId
            );

            var (_, context) = await ExternalApiService.PostAsync(
                ExternalService.BatchSchedulerService,
                RegisterJobEndpoint.RegisterScheduleJobDelayEndpoint,
                JsonConvert.SerializeObject(request),
                cancellationToken
            );

            if (!string.IsNullOrEmpty(context))
            {
                logger.LogInformation(
                    "{Action} Send Batch Job {Reservation} successfully",
                    nameof(BookingCancellationCommandHandler),
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
                nameof(BookingCancellationCommandHandler),
                bookingId,
                ex.Message
            );

            IntegrationEventOutbox eventOutbox = new()
            {
                ServiceName = DefaultValues.ServiceNameOfManager,
                EventName = request.EventName,
                JobName = request.JobName,
                JsonData = request.JsonData
            };
            EventOutboxes.Add(eventOutbox);
        }

        return false;
    }
}
