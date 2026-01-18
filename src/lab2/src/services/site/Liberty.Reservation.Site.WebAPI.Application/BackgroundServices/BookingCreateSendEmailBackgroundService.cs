using Liberty.ApplicationShared.BackgroundServices;
using Liberty.Hangfire;
using Liberty.Hangfire.Models;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Settings;
using Liberty.Reservation.Application.Templates;
using Liberty.Reservation.Site.WebAPI.Application.Web.ApiService;
using Liberty.SysIntegrationEvent.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Site.WebAPI.Application.BackgroundServices;

public record BookingCreateSendEmailJob(
    string FacilityCode,
    string SiteCode,
    string? LanguageCode,
    ReservationEntity NewBooking
);

public class BookingCreateSendEmailBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<BookingCreateSendEmailBackgroundService> logger
) : BaseBackgroundService<BookingCreateSendEmailJob>(serviceProvider, logger)
{
    protected override async Task ProcessJobAsync(
        BookingCreateSendEmailJob job,
        CancellationToken cancellationToken
    )
    {
        using var scope = ServiceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        var scopedIntegrationEventOutboxService = scopedProvider.GetRequiredService<IIntegrationEventOutboxService>();
        var mailTemplateService = scopedProvider.GetRequiredService<IMailTemplateService>();
        var mailTemplateSetting = scopedProvider.GetRequiredService<IOptions<MailTemplateSetting>>();
        var planService = scopedProvider.GetRequiredService<IPlanService>();

        var templateFormatData = await mailTemplateService.FindMailTemplateAsync(
            cancellationToken
        );

        if (templateFormatData is null)
        {
            const string errorMsg = "Mail template not found";
            logger.LogError(errorMsg);

            return;
        }

        job.NewBooking.Plan = await planService.GetPlanAtCreateBookingAsync(
            job.NewBooking.PlanId,
            cancellationToken
        );

        var eventOutboxes = await SendEmails(
            job,
            templateFormatData,
            mailTemplateSetting.Value,
            cancellationToken
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
        BookingCreateSendEmailJob job,
        TemplateFormatData templateFormatData,
        MailTemplateSetting mailTemplateSetting,
        CancellationToken cancellationToken
    )
    {
        var newReservation = job.NewBooking;
        var isTemporaryReservation = string.IsNullOrWhiteSpace(newReservation.UserCode)
            && newReservation.ReservationState == ReservationStatus.Temporary;

        var templateFormatI01 = job.LanguageCode switch
        {
            "en" => templateFormatData.CreateTemplate<Io10001Template>(IoType.IO10001En),
            _ => templateFormatData.CreateTemplate<Io10001Template>(IoType.IO10001)
        };

        var templateFormatI04 = job.LanguageCode switch
        {
            "en" => templateFormatData.CreateTemplate<Io10004EnTemplate>(IoType.IO10004En),
            _ => templateFormatData.CreateTemplate<Io10004Template>(IoType.IO10004)
        };

        var eventOutboxes = new List<IntegrationEventOutbox>();

        if (isTemporaryReservation)
        {
            var outboxesSendEmailToGuest = await SendEmailToGuest(
                job,
                mailTemplateSetting,
                templateFormatI01,
                cancellationToken
            );
            eventOutboxes.AddRange(outboxesSendEmailToGuest);
        }

        var isUserReservation = !string.IsNullOrWhiteSpace(newReservation.UserCode) && newReservation.IsOnSidePayment;
        if (!isUserReservation)
        {
            return [];
        }

        var outboxesSendEmailToUser = await SendEmailToUser(
            job,
            mailTemplateSetting,
            templateFormatI04,
            cancellationToken
        );
        eventOutboxes.AddRange(outboxesSendEmailToUser);

        var outboxesSendEmailToFacility = await SendEmailToFacility(
            job,
            mailTemplateSetting,
            templateFormatData.CreateTemplate<Io10003Template>(IoType.IO10003),
            cancellationToken
        );
        eventOutboxes.AddRange(outboxesSendEmailToFacility);

        return eventOutboxes;
    }

    private async Task<IReadOnlyList<IntegrationEventOutbox>> SendEmailToGuest(
        BookingCreateSendEmailJob job,
        MailTemplateSetting mailTemplateSetting,
        Io10001Template? mailTemplate,
        CancellationToken cancellationToken
    )
    {
        using var scope = ServiceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        var bookingSecureUrlService = scopedProvider.GetRequiredService<IBookingSecureUrlService>();

        var newReservation = job.NewBooking;
        var bookingId = newReservation.Id.ToString();
        var facilityId = newReservation.FacilityId.ToString();
        var toMail = newReservation.Reserver?.EMail;
        var applicationName = mailTemplateSetting.ApplicationName ?? string.Empty;

        if (mailTemplate is null)
        {
            const string errorMsg = $"Mail template {IoType.IO10001} not found";
            logger.LogError(errorMsg);
            return [];
        }

        var validMinutes = (int)(AppDate.GetDateTime(newReservation.CheckInDate, newReservation.CheckInTime) - DateTime.UtcNow)
            .TotalMinutes;
        var secureRequest = new BookingSecureUrlRequest(
            bookingId,
            validMinutes
        );
        var guestCode = bookingSecureUrlService.EncryptDataWithHmacSha256(secureRequest);
        var facilityCode = job.FacilityCode;
        var siteCode = job.SiteCode;
        mailTemplate.URL = mailTemplateSetting.GuestUrl?
                .Replace("{facilityCode}", facilityCode)
                .Replace("{siteCode}", siteCode)
            ?? string.Empty;

        mailTemplate.SetData(newReservation, applicationName, guestCode);
        var message = new
        {
            bookingId,
            tos = new[] { toMail },
            subject = mailTemplate.Subject,
            body = mailTemplate.Body,
            fromDisplayName = mailTemplate.FromDisplayName,
            triggerSource = MailActorTypes.Site,
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

    private async Task<IReadOnlyList<IntegrationEventOutbox>> SendEmailToUser(
        BookingCreateSendEmailJob job,
        MailTemplateSetting mailTemplateSetting,
        Io10004Template? mailTemplate,
        CancellationToken cancellationToken
    )
    {
        var newReservation = job.NewBooking;
        var bookingId = newReservation.Id.ToString();
        var facilityId = newReservation.FacilityId.ToString();
        var toMail = newReservation.Reserver?.EMail;
        var applicationName = mailTemplateSetting.ApplicationName ?? string.Empty;

        if (mailTemplate is null)
        {
            const string errorMsg = $"Mail template {IoType.IO10004} not found";
            logger.LogError(errorMsg);
            return [];
        }

        mailTemplate.URL = mailTemplateSetting.UserUrl ?? string.Empty;
        mailTemplate.SetData(newReservation, applicationName);

        var message = new
        {
            bookingId,
            tos = new[] { toMail },
            subject = mailTemplate.Subject,
            body = mailTemplate.Body,
            fromDisplayName = mailTemplate.FromDisplayName,
            triggerSource = MailActorTypes.Site,
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

    private async Task<IReadOnlyList<IntegrationEventOutbox>> SendEmailToFacility(
        BookingCreateSendEmailJob job,
        MailTemplateSetting mailTemplateSetting,
        Io10003Template? mailTemplate,
        CancellationToken cancellationToken
    )
    {
        using var scope = ServiceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        var facilityService = scopedProvider.GetRequiredService<IFacilityService>();

        var newReservation = job.NewBooking;
        var bookingId = newReservation.Id.ToString();
        var facilityId = newReservation.FacilityId;
        var (facilityFax, facilityMail) = await facilityService.GetSystemMailAddressAsync(
            facilityId,
            cancellationToken
        );
        if (string.IsNullOrEmpty(facilityMail))
        {
            return [];
        }

        if (mailTemplate is null)
        {
            const string errorMsg = $"Mail template {IoType.IO10003} not found";
            logger.LogError(errorMsg);
            return [];
        }

        var facilityCode = job.FacilityCode;
        mailTemplate.URL = mailTemplateSetting.ManagerUrl?.Replace("{facilityCode}", facilityCode);
        var applicationName = mailTemplateSetting.ApplicationName ?? string.Empty;
        mailTemplate.SetData(newReservation, applicationName);
        var message = new
        {
            bookingId,
            tos = new[] { facilityMail },
            subject = mailTemplate.Subject,
            body = mailTemplate.Body,
            isSendFax = !string.IsNullOrWhiteSpace(facilityFax),
            faxNumber = facilityFax,
            fromDisplayName = mailTemplate.FromDisplayName,
            triggerSource = MailActorTypes.Site,
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
                nameof(BookingCreateSendEmailBackgroundService),
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
                    nameof(BookingCreateSendEmailBackgroundService),
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
                nameof(BookingCreateSendEmailBackgroundService),
                bookingId,
                ex.Message
            );

            IntegrationEventOutbox eventOutbox = new()
            {
                ServiceName = DefaultValues.ServiceNameOfSite,
                EventName = request.EventName,
                JobName = request.JobName,
                JsonData = request.JsonData
            };
            eventOutboxes.Add(eventOutbox);
        }

        return eventOutboxes;
    }
}
