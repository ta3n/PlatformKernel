using Liberty.GmoPaymentGateway.Exceptions;
using Liberty.GmoPaymentGateway.Models.Requests;
using Liberty.GmoPaymentGateway.Services;
using Liberty.Hangfire;
using Liberty.Hangfire.Models;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.Settings;
using Liberty.Reservation.Application.Templates;
using Liberty.Reservation.Application.UseCases.Commands.BookingReservation;
using Liberty.Reservation.User.Application.Auth;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;
using Liberty.Reservation.User.WebAPI.Application.Web.ApiService;
using Liberty.SysException.Exceptions;
using Liberty.SysIntegrationEvent.Events;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;

public class BookingCancellationCommandHandler(
    ILogger<BookingCancellationCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ISecurityContextAccessor securityContextAccessor,
    IServiceProvider serviceProvider
) : UpdateCommandHandlerBase<BookingCancellationCommand, long>(unitOfWork, null!)
{
    private IBookingCheckAvailableService BookingCheckAvailableService
        => serviceProvider.GetRequiredService<IBookingCheckAvailableService>();

    private IBookingReservationService BookingReservationService
        => serviceProvider.GetRequiredService<IBookingReservationService>();

    private IGmoPaymentGatewayService GmoPaymentGatewayService
        => serviceProvider.GetRequiredService<IGmoPaymentGatewayService>();

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

    private List<IntegrationEventOutbox> EventOutboxes { get; set; } = [];

    protected override async Task<long> HandleAsync(
        BookingCancellationCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var userCode = securityContextAccessor.ApplicationUserKey;

        var existingReservation = await BookingCheckAvailableService.GetReservationByUserAsync(
            payload.Id ?? 0,
            userCode,
            cancellationToken
        );

        if (!existingReservation.IsReserved || !existingReservation.CanModifyByUser())
        {
            throw new ReservationInvalidException(string.Empty);
        }

        await ValidateGlobalOnlinePayment(existingReservation, cancellationToken);

        var languageCode = existingReservation.BookingData?.LanguageCode ?? securityContextAccessor.GetLanguageCode();

        var cancelledDateTime = DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset);
        var cancellationPrice = BookingReservationService.GetCancellationPrice(
            cancelledDateTime,
            existingReservation
        );

        var reservationState = string.IsNullOrEmpty(userCode) ? ReservationStatus.GuestCanceled : ReservationStatus.UserCanceled;

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var (_, cancellationFeeResponse) = await mediator.Send(
                new ReservationGetCancellationFeeQuery(request.Payload.Id ?? 0),
                cancellationToken
            );

            var bookingResponse = await mediator.Send(
                new BookingAbortCommand(
                    existingReservation,
                    reservationState,
                    cancellationPrice,
                    cancellationFeeResponse.RateFee
                ) { Payload = payload },
                cancellationToken
            );

            if (existingReservation.IsOnlinePayment)
            {
                _ = await OnlinePaymentRefundAsync(
                    bookingResponse.BookingId,
                    bookingResponse.CancellationPrice
                );
            }

            await UnitOfWork.CommitAsync(cancellationToken);

            SetCancelInfoToReservation(existingReservation, bookingResponse);
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

            return bookingResponse.BookingId;
        }
        catch (Exception ex) when (ex is PaymentCancelException)
        {
            logger.LogError(ex, "Online payment refund reservation cancel failed: {Message}", ex.Message);
            await HandlerRefundErrorAsync(
                existingReservation,
                cancelledDateTime,
                cancellationPrice,
                cancellationToken
            );

            throw new AppLibertyException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Cancellation reservation failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private static void SetCancelInfoToReservation(
        Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation existingReservation,
        BookingAbortResponse request
    )
    {
        existingReservation.CancellationPrice = request.CancellationPrice;
        existingReservation.CancelRateFee = request.CancellationRate;
        existingReservation.CancelledDateTime = request.CancellationTime;
    }

    private async Task ValidateGlobalOnlinePayment(
        Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation existingReservation,
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
            "{Action} - Successfully online payment refund {ReservationId} {OderId} {TranId}",
            nameof(OnlinePaymentRefundAsync),
            reservationId,
            orderId,
            responseCancel.TranId
        );

        return responseCancel.TranId;
    }

    private async Task HandlerRefundErrorAsync(
        Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation existingReservation,
        DateTime cancelledDateTime,
        decimal cancellationPrice,
        CancellationToken cancellationToken
    )
    {
        UnitOfWork.UpdateState(existingReservation, EntityState.Detached);
        existingReservation.ReservationState = ReservationStatus.UserCanceled;
        existingReservation.CancelledDateTime = cancelledDateTime;
        existingReservation.CancellationPrice = cancellationPrice;
        UnitOfWork.UpdateState(existingReservation, EntityState.Modified);
        await UnitOfWork.CommitAsync(cancellationToken);
    }

    private async Task<bool> SendEmails(
        Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation existingReservation,
        string? languageCode,
        CancellationToken cancellationToken
    )
    {
        var templateFormatData = await BookingReservationService.FindMailTemplateAsync(
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

    private async Task<bool> SendEmailToUser(
        Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation existingReservation,
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
        mailTemplate.SetData(existingReservation, applicationName, ReservationOperationTypes.User);
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
        Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation existingReservation,
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

        var facilityCode = existingReservation.Facility?.Code;
        mailTemplate.URL = MailTemplateSetting.Value.ManagerUrl?.Replace("{facilityCode}", facilityCode);
        mailTemplate.SetData(
            existingReservation,
            applicationName,
            ReservationOperationTypes.User
        );
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

    private async Task<bool> SendEmailToGuest(
        Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation existingReservation,
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
            ReservationOperationTypes.User
        );

        var message = new
        {
            bookingId,
            tos = new[] { toMail },
            subject = mailTemplate.Subject,
            body = mailTemplate.Body
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
