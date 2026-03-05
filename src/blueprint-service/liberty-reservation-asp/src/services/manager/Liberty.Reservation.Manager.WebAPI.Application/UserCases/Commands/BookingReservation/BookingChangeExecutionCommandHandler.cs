using Liberty.GmoPaymentGateway.Exceptions;
using Liberty.Hangfire;
using Liberty.Hangfire.Models;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Settings;
using Liberty.Reservation.Application.Templates;
using Liberty.Reservation.Application.UseCases.Commands.BookingReservation;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.Web.ApiService;
using Liberty.SysIntegrationEvent.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BookingReservation;

public class BookingChangeExecutionCommandHandler(
    ILogger<BookingChangeExecutionCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ISecurityContextAccessor securityContextAccessor,
    IServiceProvider serviceProvider
) : UpdateCommandHandlerBase<BookingChangeExecutionCommand, long>(unitOfWork, null!)
{
    private IBookingCheckAvailableService BookingCheckAvailableService
        => serviceProvider.GetRequiredService<IBookingCheckAvailableService>();

    private IBookingReservationService BookingReservationService
        => serviceProvider.GetRequiredService<IBookingReservationService>();

    private IBookingCheckModifyInPriceService BookingCheckModifyInPriceService
        => serviceProvider.GetRequiredService<IBookingCheckModifyInPriceService>();

    private IExternalApiService ExternalApiService
        => serviceProvider.GetRequiredService<IExternalApiService>();

    private IBookingSecureUrlService BookingSecureUrlService
        => serviceProvider.GetRequiredService<IBookingSecureUrlService>();

    private IIntegrationEventOutboxService IntegrationEventOutboxService
        => serviceProvider.GetRequiredService<IIntegrationEventOutboxService>();

    private IOptions<MailTemplateSetting> MailTemplateSetting
        => serviceProvider.GetRequiredService<IOptions<MailTemplateSetting>>();

    private IBookingManagerModificationCheckerService BookingModificationCheckerService
        => serviceProvider.GetRequiredService<IBookingManagerModificationCheckerService>();

    private IBookingPaymentRestrictionService BookingPaymentRestrictionService
        => serviceProvider.GetRequiredService<IBookingPaymentRestrictionService>();

    private IFacilityService FacilityService
        => serviceProvider.GetRequiredService<IFacilityService>();

    private IBookingOnlinePaymentService BookingOnlinePaymentService
        => serviceProvider.GetRequiredService<IBookingOnlinePaymentService>();

    private IGmoChangeTranReportService GmoChangeTranReportService
        => serviceProvider.GetRequiredService<IGmoChangeTranReportService>();

    private List<IntegrationEventOutbox> EventOutboxes { get; set; } = [];

    protected override async Task<long> HandleAsync(
        BookingChangeExecutionCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var facilityId = securityContextAccessor.FacilityKey;

        var existingReservation = await BookingCheckAvailableService.GetReservationByFacilityAsync(
            payload.Id,
            facilityId,
            cancellationToken
        );

        EnsureDayUseNightLimit(existingReservation.BookingData?.Plan.DayUse, payload.NumberOfNights);
        EnsureReservationIsReserved(existingReservation);
        EnsureManagerCanModify(existingReservation);

        var languageCode = existingReservation.BookingData?.LanguageCode ?? securityContextAccessor.GetLanguageCode();

        var isModifyInPrice = await BookingCheckModifyInPriceService.IsModifyInPriceAsync(
            existingReservation,
            payload,
            cancellationToken
        );

        await ValidatePaymentRestrictionsAsync(existingReservation, isModifyInPrice, cancellationToken);
        await EnsureOnsiteAllowedOrThrowAsync(existingReservation, isModifyInPrice, cancellationToken);

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var bookingResponse = await mediator.Send(
                new BookingAdjustCommand(
                    existingReservation,
                    isModifyInPrice ? ReservationStatus.ManagerModified : ReservationStatus.Modified,
                    isModifyInPrice
                )
                {
                    Payload = payload,
                    IsNotCheckValidDateLimit = true
                },
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

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

            if (!existingReservation.IsOnlinePayment)
            {
                throw;
            }

            if (ex is OnlinePaymentChangeOrderException onlineEx)
            {
                throw new OnlinePaymentChangeOrderException(onlineEx.ErrorCaption);
            }

            var (existingReport, orderId) = await HasChangeTranAmountAsync(
                existingReservation.Id,
                cancellationToken
            );

            if (!existingReport)
            {
                throw new OnlinePaymentUpdateChangeAmountException(orderId);
            }

            var (resultRollback, orderOfRollback) = await HandlerRollbackChangeAmountAsync(existingReservation, cancellationToken);

            if (string.IsNullOrEmpty(resultRollback))
            {
                throw new OnlinePaymentRollbackChangeOrderException(orderOfRollback);
            }

            throw new OnlinePaymentUpdateChangeAmountException(orderId);
        }
    }

    private static void EnsureDayUseNightLimit(
        bool? dayUse,
        int? numberOfNights
    )
    {
        if (dayUse == true && numberOfNights is not 1)
        {
            throw new PlanUseDayInvalidNightLimitException();
        }
    }

    private static void EnsureReservationIsReserved(
        ReservationEntity existing
    )
    {
        if (!existing.IsReserved)
        {
            throw new ReservationInvalidException(string.Empty);
        }
    }

    private void EnsureManagerCanModify(
        ReservationEntity existing
    )
    {
        var allowed = BookingModificationCheckerService.CanBookingChangeModify(
            existing.CheckInDate,
            existing.IsNoShow,
            existing.ReservationState
        );

        if (!allowed)
        {
            throw new ReservationChangeNotAllowedException();
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

    private async Task<(string? transactionId, string? orderId)> HandlerRollbackChangeAmountAsync(
        ReservationEntity existingReservation,
        CancellationToken cancellationToken
    )
    {
        var (transactionId, orderId) = await BookingOnlinePaymentService.OnlinePaymentChangeAmountAsync(
            existingReservation.Id,
            existingReservation.BookingData?.AllTotalPrice ?? 0,
            true,
            cancellationToken
        );
        return (transactionId, orderId);
    }

    private async Task<(bool isExisting, string? orderId)> HasChangeTranAmountAsync(
        long reservationId,
        CancellationToken cancellationToken = default
    )
    {
        return await GmoChangeTranReportService.IsExistAsync(reservationId, cancellationToken);
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
        var guestCode = BookingSecureUrlService.EncryptDataWithHmacSha256(secureRequest);
        mailTemplate.URL = MailTemplateSetting.Value.GuestUrl?.Replace("{guestCode}", guestCode);
        mailTemplate.SetData(
            newReservation,
            applicationName,
            ReservationOperationTypes.Facility
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

    private async Task<bool> SendEmailToUser(
        ReservationEntity existingReservation,
        ReservationEntity newReservation,
        Io10008Template? mailTemplate,
        CancellationToken cancellationToken
    )
    {
        var bookingId = newReservation.Id.ToString();
        var facilityId = existingReservation.FacilityId.ToString();
        var toMail = newReservation.Reserver?.EMail;
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
            ReservationOperationTypes.Facility
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

        var facilityCode = securityContextAccessor.GetFacilityCode();
        mailTemplate.URL = MailTemplateSetting.Value.ManagerUrl?.Replace("{facilityCode}", facilityCode);
        mailTemplate.SetData(newReservation, existingReservation, applicationName, ReservationOperationTypes.Facility);
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
                nameof(BookingChangeExecutionCommandHandler),
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
