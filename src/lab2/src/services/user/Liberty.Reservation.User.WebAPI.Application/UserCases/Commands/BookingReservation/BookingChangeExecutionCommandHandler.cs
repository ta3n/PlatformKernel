using Liberty.GmoPaymentGateway.Exceptions;
using Liberty.Hangfire;
using Liberty.Hangfire.Models;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Settings;
using Liberty.Reservation.Application.Templates;
using Liberty.Reservation.Application.UseCases.Commands.BookingReservation;
using Liberty.Reservation.User.Application.Auth;
using Liberty.Reservation.User.Application.Domains.Services.Interfaces;
using Liberty.Reservation.User.Application.Exceptions;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Events.Booking;
using Liberty.Reservation.User.WebAPI.Application.Web.ApiService;
using Liberty.SysIntegrationEvent.Events;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;

public class BookingChangeExecutionCommandHandler(
    ILogger<BookingChangeExecutionCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediator mediator,
    ISecurityContextAccessor securityContextAccessor,
    IServiceProvider serviceProvider
) : UpdateCommandWithAuditEventHandlerBase<BookingChangeExecutionCommand, long>(unitOfWork, mapper, mediator)
{
    private IBookingCheckAvailableService BookingCheckAvailableService
        => serviceProvider.GetRequiredService<IBookingCheckAvailableService>();

    private IBookingReservationService BookingReservationService
        => serviceProvider.GetRequiredService<IBookingReservationService>();

    private IBookingCheckModifyInPriceService BookingCheckModifyInPriceService
        => serviceProvider.GetRequiredService<IBookingCheckModifyInPriceService>();

    private IIntegrationEventOutboxService IntegrationEventOutboxService
        => serviceProvider.GetRequiredService<IIntegrationEventOutboxService>();

    private IOptions<MailTemplateSetting> MailTemplateSetting
        => serviceProvider.GetRequiredService<IOptions<MailTemplateSetting>>();

    private IBookingPaymentRestrictionService BookingPaymentRestrictionService
        => serviceProvider.GetRequiredService<IBookingPaymentRestrictionService>();

    private IFacilityService FacilityService
        => serviceProvider.GetRequiredService<IFacilityService>();

    private List<IntegrationEventOutbox> EventOutboxes { get; set; } = [];

    protected override async Task<long> HandleAsync(
        BookingChangeExecutionCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var userCode = securityContextAccessor.ApplicationUserKey;

        var existingReservation = await BookingCheckAvailableService.GetReservationByUserAsync(
            payload.Id,
            userCode,
            cancellationToken
        );

        EnsureUseDayNightLimit(existingReservation.BookingData?.Plan.DayUse, payload.NumberOfNights);
        EnsureUserCanModify(existingReservation);
        if (!existingReservation.CanModifyByUser(false))
        {
            throw new ReservationOutOfDateException();
        }

        EnsureExtendedStayLimit(
            payload.NumberOfNights,
            existingReservation.RestNumber,
            existingReservation.Facility!.IsExtendedStayOnModify
        );

        var languageCode = existingReservation.BookingData?.LanguageCode ?? securityContextAccessor.GetLanguageCode();

        var isModifyInPrice = await BookingCheckModifyInPriceService.IsModifyInPriceAsync(
            existingReservation,
            payload,
            cancellationToken
        );

        await ValidatePaymentRestrictionsAsync(existingReservation, isModifyInPrice, cancellationToken);
        await EnsureOnsiteAllowedOrThrowAsync(existingReservation, isModifyInPrice, cancellationToken);

        var reservationState = string.IsNullOrEmpty(userCode) ? ReservationStatus.GuestModified : ReservationStatus.UserModified;

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var bookingResponse = await Mediator.Send(
                new BookingAdjustCommand(
                    existingReservation,
                    reservationState,
                    isModifyInPrice
                ) { Payload = payload },
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            AuditEventData = new BookingUpdatedEvent
            {
                Id = bookingResponse.Id,
                AggregateCode = bookingResponse.Code ?? string.Empty,
                Request = request,
                UserCode = securityContextAccessor.ApplicationUserKey,
                FacilityId = bookingResponse.FacilityId,
                SiteId = bookingResponse.SiteId,
                OldId = existingReservation.Id,
                LanguageCode = securityContextAccessor.GetLanguageCode()
            };
            _ = await SendEmails(
                existingReservation,
                bookingResponse,
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

            return bookingResponse.Id;
        }
        catch (Exception ex)
        {
            await UnitOfWork.RollbackAsync(cancellationToken);

            if (!existingReservation.IsOnlinePayment || !isModifyInPrice)
            {
                throw;
            }

            if (ex is OnlinePaymentChangeOrderException onlineEx)
            {
                throw new OnlinePaymentChangeOrderException(onlineEx.ErrorCaption);
            }

            throw new OnlinePaymentUpdateChangeAmountException(existingReservation.Code);
        }
    }

    private static void EnsureExtendedStayLimit(
        int numberOfNights,
        int existingNumberOfNights,
        bool isExtendedStayOnModify = false
    )
    {
        if (isExtendedStayOnModify)
        {
            return;
        }

        if (numberOfNights > existingNumberOfNights)
        {
            throw new FacilityNotAllowExtendedStayException();
        }
    }

    private static void EnsureUseDayNightLimit(
        bool? dayUse,
        int? numberOfNights
    )
    {
        if (dayUse == true && numberOfNights is not 1)
        {
            throw new BookingInvalidNightNumberOfUseDayException();
        }
    }

    private static void EnsureUserCanModify(
        ReservationEntity existing
    )
    {
        if (!existing.IsReserved || !existing.CanModifyByUser(false))
        {
            throw new ReservationInvalidException(string.Empty);
        }
    }

    private async Task ValidatePaymentRestrictionsAsync(
        ReservationEntity existing,
        bool isModifyInPrice,
        CancellationToken ct
    )
    {
        await BookingPaymentRestrictionService.ValidateGlobalOnlinePaymentAsync(
            existing,
            isModifyInPrice,
            ct
        );
    }

    private async Task EnsureOnsiteAllowedOrThrowAsync(
        ReservationEntity existing,
        bool isModifyInPrice,
        CancellationToken ct
    )
    {
        var allowOnSiteLocal = await FacilityService.CheckPaymentOnSitePaymentAvailableAsync(
            existing.FacilityId,
            ct
        );

        if (!allowOnSiteLocal && isModifyInPrice && existing.IsOnSidePayment)
        {
            throw new PlanPaymentOnSiteNotAvailableException();
        }
    }

    private async Task<bool> SendEmails(
        ReservationEntity existingReservation,
        ReservationEntity newReservation,
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

        var templateFormatI08 = languageCode switch
        {
            "en" => templateFormatData.CreateTemplate<Io10008EnTemplate>(IoType.IO10008En),
            _ => templateFormatData.CreateTemplate<Io10008Template>(IoType.IO10008)
        };
        var templateFormatI010010 = languageCode switch
        {
            "en" => templateFormatData.CreateTemplate<Io10010EnTemplate>(IoType.IO10010En),
            _ => templateFormatData.CreateTemplate<Io10010Template>(IoType.IO10010)
        };

        if (!string.IsNullOrWhiteSpace(existingReservation.UserCode))
        {
            _ = await SendEmailToUser(
                existingReservation,
                newReservation,
                templateFormatI08,
                cancellationToken
            );
        }
        else
        {
            _ = await SendEmailToGuest(
                existingReservation,
                newReservation,
                templateFormatI010010,
                cancellationToken
            );
        }

        _ = await SendEmailToFacility(
            existingReservation,
            newReservation,
            templateFormatData.CreateTemplate<Io10007Template>(IoType.IO10007),
            cancellationToken
        );

        return true;
    }

    private async Task<bool> SendEmailToUser(
        ReservationEntity existingReservation,
        ReservationEntity newReservation,
        Io10008Template? mailTemplate,
        CancellationToken cancellationToken
    )
    {
        var bookingId = existingReservation.Id.ToString();
        var facilityId = existingReservation.FacilityId.ToString();
        var toMail = existingReservation.Reserver?.EMail;
        var applicationName = MailTemplateSetting.Value.ApplicationName ?? string.Empty;

        if (mailTemplate is null)
        {
            const string errorMsg = $"Mail template {IoType.IO10008} not found";
            logger.LogError(errorMsg);
            return false;
        }

        mailTemplate.URL = MailTemplateSetting.Value.UserUrl;
        mailTemplate.SetData(
            newReservation,
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
            fromDisplayName = mailTemplate.FromDisplayName,
            triggerSource = MailActorTypes.User,
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

    private async Task<bool> SendEmailToFacility(
        ReservationEntity existingReservation,
        ReservationEntity newReservation,
        Io10007Template? mailTemplate,
        CancellationToken cancellationToken
    )
    {
        var bookingId = newReservation.Id.ToString();
        var facilityId = existingReservation.FacilityId.ToString();
        var toMail = existingReservation.Facility?.Meta?.SystemEMail;
        var toFax = existingReservation.Facility?.Fax;
        var isSendFax = !string.IsNullOrWhiteSpace(toFax) && existingReservation.Facility?.UseFax is true;
        var applicationName = MailTemplateSetting.Value.ApplicationName ?? string.Empty;

        if (mailTemplate is null)
        {
            const string errorMsg = $"Mail template {IoType.IO10007} not found";
            logger.LogError(errorMsg);
            return false;
        }

        var facilityCode = existingReservation.Facility?.Code;
        mailTemplate.URL = MailTemplateSetting.Value.ManagerUrl?.Replace("{facilityCode}", facilityCode);
        mailTemplate.SetData(
            newReservation,
            existingReservation,
            applicationName,
            ReservationOperationTypes.User
        );

        var isUserReservation = !string.IsNullOrWhiteSpace(existingReservation.UserCode);

        var message = new
        {
            bookingId,
            tos = new[] { toMail },
            subject = mailTemplate.Subject,
            body = mailTemplate.Body,
            isSendFax,
            faxNumber = toFax,
            fromDisplayName = mailTemplate.FromDisplayName,
            triggerSource = isUserReservation ? MailActorTypes.User : MailActorTypes.Guest,
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

    private async Task<bool> SendEmailToGuest(
        ReservationEntity existingReservation,
        ReservationEntity newReservation,
        Io10010Template? mailTemplate,
        CancellationToken cancellationToken
    )
    {
        var bookingId = newReservation.Id.ToString();
        var facilityId = existingReservation.FacilityId.ToString();
        var toMail = newReservation.Reserver?.EMail;
        var applicationName = MailTemplateSetting.Value.ApplicationName ?? string.Empty;

        if (mailTemplate is null)
        {
            const string errorMsg = $"Mail template {IoType.IO10010} not found";
            logger.LogError(errorMsg);
            return false;
        }

        var validMinutes = (int)(AppDate.GetDateTime(newReservation.CheckInDate, newReservation.CheckInTime) - DateTime.UtcNow)
            .TotalMinutes;
        var secureRequest = new BookingSecureUrlRequest(
            bookingId,
            validMinutes
        );

        var bookingSecureUrlService = serviceProvider.GetRequiredService<IBookingSecureUrlService>();
        var guestCode = bookingSecureUrlService.EncryptDataWithHmacSha256(secureRequest);
        mailTemplate.URL = MailTemplateSetting.Value.GuestUrl?.Replace("{guestCode}", guestCode);
        mailTemplate.SetData(
            newReservation,
            applicationName,
            ReservationOperationTypes.User
        );

        var message = new
        {
            bookingId,
            tos = new[] { toMail },
            subject = mailTemplate.Subject,
            body = mailTemplate.Body,
            fromDisplayName = mailTemplate.FromDisplayName,
            triggerSource = MailActorTypes.Guest,
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
                nameof(BookingChangeExecutionCommandHandler),
                bookingId
            );

            var externalApiService = serviceProvider.GetRequiredService<IExternalApiService>();
            var (_, context) = await externalApiService.PostAsync(
                ExternalService.BatchSchedulerService,
                RegisterJobEndpoint.RegisterScheduleJobDelayEndpoint,
                JsonConvert.SerializeObject(request),
                cancellationToken
            );

            if (!string.IsNullOrEmpty(context))
            {
                logger.LogInformation(
                    "{Action} Send Batch Job {Reservation} successfully",
                    nameof(BookingChangeExecutionCommandHandler),
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
                nameof(BookingChangeExecutionCommandHandler),
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
