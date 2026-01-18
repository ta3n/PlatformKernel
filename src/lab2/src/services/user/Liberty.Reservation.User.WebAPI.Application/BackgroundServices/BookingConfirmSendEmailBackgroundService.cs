using Liberty.ApplicationShared.BackgroundServices;
using Liberty.Hangfire;
using Liberty.Hangfire.Models;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Settings;
using Liberty.Reservation.Application.Templates;
using Liberty.Reservation.User.Application.Domains.Services.Interfaces;
using Liberty.Reservation.User.WebAPI.Application.Web.ApiService;
using Liberty.SysIntegrationEvent.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.User.WebAPI.Application.BackgroundServices;

public record BookingConfirmSendEmailJob(
    long BookingId,
    string? UserCode,
    string? GuestCode,
    string? LanguageCode
);

public class BookingConfirmSendEmailBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<BookingConfirmSendEmailBackgroundService> logger
) : BaseBackgroundService<BookingConfirmSendEmailJob>(serviceProvider, logger)
{
    protected override async Task ProcessJobAsync(
        BookingConfirmSendEmailJob job,
        CancellationToken cancellationToken
    )
    {
        using var scope = ServiceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        var scopedIntegrationEventOutboxService = scopedProvider.GetRequiredService<IIntegrationEventOutboxService>();
        var mailTemplateService = scopedProvider.GetRequiredService<IMailTemplateService>();
        var mailTemplateSetting = scopedProvider.GetRequiredService<IOptions<MailTemplateSetting>>();
        var bookingReservationService = scopedProvider.GetRequiredService<IBookingCheckAvailableService>();

        var templateFormatData = await mailTemplateService.FindMailTemplateAsync(
            cancellationToken
        );

        if (templateFormatData is null)
        {
            const string errorMsg = "Mail template not found";
            logger.LogError(errorMsg);

            return;
        }

        var existingReservation = await bookingReservationService.GetReservationByUserAsync(
            job.BookingId,
            job.UserCode,
            CancellationToken.None
        );

        var eventOutboxes = await SendEmails(
            existingReservation,
            job.GuestCode,
            job.LanguageCode,
            templateFormatData,
            mailTemplateSetting.Value,
            CancellationToken.None
        );

        if (eventOutboxes is { Count: > 0 })
        {
            await scopedIntegrationEventOutboxService.CreateRangeAsync(
                eventOutboxes,
                true,
                CancellationToken.None
            );
        }
    }

    private async Task<IReadOnlyList<IntegrationEventOutbox>> SendEmails(
        ReservationEntity existingReservation,
        string? guestCode,
        string? languageCode,
        TemplateFormatData templateFormatData,
        MailTemplateSetting mailTemplateSetting,
        CancellationToken cancellationToken
    )
    {
        var templateFormatI010012 = languageCode switch
        {
            "en" => templateFormatData.CreateTemplate<Io10012EnTemplate>(IoType.IO10012En),
            _ => templateFormatData.CreateTemplate<Io10012Template>(IoType.IO10012)
        };

        var eventOutboxes = new List<IntegrationEventOutbox>();

        var outboxesSendEmailToGuest = await SendEmailToGuest(
            existingReservation,
            mailTemplateSetting,
            templateFormatI010012,
            guestCode,
            cancellationToken
        );
        eventOutboxes.AddRange(outboxesSendEmailToGuest);

        var outboxesSendEmailToFacility = await SendEmailToFacility(
            existingReservation,
            mailTemplateSetting,
            templateFormatData.CreateTemplate<Io10003Template>(IoType.IO10003),
            cancellationToken
        );
        eventOutboxes.AddRange(outboxesSendEmailToFacility);

        return eventOutboxes;
    }

    private async Task<IReadOnlyList<IntegrationEventOutbox>> SendEmailToGuest(
        ReservationEntity existingReservation,
        MailTemplateSetting mailTemplateSetting,
        Io10012Template? mailTemplate,
        string? guestCode,
        CancellationToken cancellationToken
    )
    {
        var bookingId = existingReservation.Id.ToString();
        var facilityId = existingReservation.FacilityId.ToString();
        var toMail = existingReservation.Reserver?.EMail;
        var applicationName = mailTemplateSetting.ApplicationName ?? string.Empty;

        if (mailTemplate is null)
        {
            const string errorMsg = $"Mail template {IoType.IO10012} not found";
            logger.LogError(errorMsg);
            return [];
        }

        mailTemplate.URL = mailTemplateSetting.GuestUrl?.Replace("{guestCode}", guestCode);
        mailTemplate.SetData(existingReservation, applicationName);
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

    private async Task<IReadOnlyList<IntegrationEventOutbox>> SendEmailToFacility(
        ReservationEntity existingReservation,
        MailTemplateSetting mailTemplateSetting,
        Io10003Template? mailTemplate,
        CancellationToken cancellationToken
    )
    {
        var bookingId = existingReservation.Id.ToString();
        var facilityId = existingReservation.FacilityId.ToString();
        var toMail = existingReservation.Facility?.Meta?.SystemEMail;
        var toFax = existingReservation.Facility?.Fax;
        var isSendFax = !string.IsNullOrWhiteSpace(toFax) && existingReservation.Facility?.UseFax is true;
        var applicationName = mailTemplateSetting.ApplicationName ?? string.Empty;

        if (mailTemplate is null)
        {
            const string errorMsg = $"Mail template {IoType.IO10003} not found";
            logger.LogError(errorMsg);
            return [];
        }

        var facilityCode = existingReservation.Facility?.Code;
        mailTemplate.URL = mailTemplateSetting.ManagerUrl?.Replace("{facilityCode}", facilityCode);
        mailTemplate.SetData(existingReservation, applicationName);
        var message = new
        {
            bookingId,
            tos = new[] { toMail },
            subject = mailTemplate.Subject,
            body = mailTemplate.Body,
            isSendFax,
            faxNumber = toFax,
            fromDisplayName = mailTemplate.FromDisplayName,
            triggerSource = MailActorTypes.Guest,
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

    private async Task<IReadOnlyList<IntegrationEventOutbox>> RegisterScheduleJobAsync(
        string bookingId,
        RegisterScheduleJobDelayRequest request,
        CancellationToken cancellationToken = default
    )
    {
        using var scope = ServiceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        var externalApiService = scopedProvider.GetRequiredService<IExternalApiService>();

        var eventOutboxes = new List<IntegrationEventOutbox>();

        try
        {
            logger.LogInformation(
                "{Action} Send Batch Job {Reservation} start",
                nameof(BookingConfirmSendEmailBackgroundService),
                bookingId
            );

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
                    nameof(BookingConfirmSendEmailBackgroundService),
                    bookingId
                );
                return [];
            }
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "{Action} Send Batch Job {Reservation} error {Message}",
                nameof(BookingConfirmSendEmailBackgroundService),
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
            eventOutboxes.Add(eventOutbox);
        }

        return eventOutboxes;
    }
}
