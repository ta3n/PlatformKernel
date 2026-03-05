using Liberty.Hangfire;
using Liberty.Hangfire.Models;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Settings;
using Liberty.Reservation.Application.Templates;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.Web.ApiService;
using Liberty.SysIntegrationEvent.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BookingReservation;

public class BookingNoShowCommandHandler(
    ILogger<BookingNoShowCommandHandler> logger,
    IUnitOfWork unitOfWork,
    ISecurityContextAccessor securityContextAccessor,
    IServiceProvider serviceProvider
) : UpdateCommandHandlerBase<BookingNoShowCommand, long>(unitOfWork, null!)
{
    private IBookingCheckAvailableService BookingCheckAvailableService
        => serviceProvider.GetRequiredService<IBookingCheckAvailableService>();

    private IBookingReservationService BookingReservationService
        => serviceProvider.GetRequiredService<IBookingReservationService>();

    private IMailTemplateService MailTemplateService
        => serviceProvider.GetRequiredService<IMailTemplateService>();

    private IExternalApiService ExternalApiService
        => serviceProvider.GetRequiredService<IExternalApiService>();

    private IBookingSecureUrlService BookingSecureUrlService
        => serviceProvider.GetRequiredService<IBookingSecureUrlService>();

    private IIntegrationEventOutboxService IntegrationEventOutboxService
        => serviceProvider.GetRequiredService<IIntegrationEventOutboxService>();

    private List<IntegrationEventOutbox> EventOutboxes { get; set; } = [];

    private IOptions<MailTemplateSetting> MailTemplateSetting
        => serviceProvider.GetRequiredService<IOptions<MailTemplateSetting>>();

    private IBookingManagerModificationCheckerService BookingModificationCheckerService
        => serviceProvider.GetRequiredService<IBookingManagerModificationCheckerService>();

    protected override async Task<long> HandleAsync(
        BookingNoShowCommand request,
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

        var canManagerModify = BookingModificationCheckerService.CanNoShowModify(
            existingReservation.CheckInTime ?? TimeSpan.Zero,
            existingReservation.CheckInDate,
            existingReservation.CheckOutDate,
            existingReservation.IsNoShow,
            existingReservation.ReservationState
        );
        if (!canManagerModify)
        {
            throw new ReservationChangeNotAllowedException();
        }

        var languageCode = existingReservation.BookingData?.LanguageCode ?? securityContextAccessor.GetLanguageCode();

        var dateNow = DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset);
        var checkInTime = existingReservation.CheckInTime?.TotalDays < 1
            ? existingReservation.CheckInTime ?? TimeSpan.Zero
            : new TimeSpan(23, 59, 59);
        var checkInDateTime = AppDate
            .GetDateTime(existingReservation.CheckInDate)
            .Add(checkInTime);
        if (dateNow < checkInDateTime)
        {
            throw new ReservationNoAllowNoShowException();
        }

        var editReservation = new Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation
        {
            Id = existingReservation.Id,
            NoShowDateTime = DateTime.UtcNow,
            IsNoShow = true,
            NoShowReason = payload.Reason,
            UpdateCount = existingReservation.UpdateCount
        };

        try
        {
            var bookingResponse = await BookingReservationService.NoShowAsync(
                editReservation,
                true,
                cancellationToken
            );

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

            return bookingResponse.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "NoShow reservation failed: {Message}", ex.Message);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private async Task<bool> SendEmails(
        Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation existingReservation,
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

        var templateFormatI010014 = languageCode switch
        {
            "en" => templateFormatData.CreateTemplate<Io10014EnTemplate>(IoType.IO10014En),
            _ => templateFormatData.CreateTemplate<Io10014Template>(IoType.IO10014)
        };
        var templateFormatI010015 = languageCode switch
        {
            "en" => templateFormatData.CreateTemplate<Io10015EnTemplate>(IoType.IO10015En),
            _ => templateFormatData.CreateTemplate<Io10015Template>(IoType.IO10015)
        };
        if (!string.IsNullOrWhiteSpace(existingReservation.UserCode))
        {
            _ = await SendEmailToUser(
                existingReservation,
                templateFormatI010014,
                cancellationToken
            );
        }
        else
        {
            _ = await SendEmailToGuest(
                existingReservation,
                templateFormatI010015,
                cancellationToken
            );
        }

        _ = await SendEmailToFacility(
            existingReservation,
            templateFormatData.CreateTemplate<Io10013Template>(IoType.IO10013),
            cancellationToken
        );

        return true;
    }

    private async Task<bool> SendEmailToGuest(
        Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation existingReservation,
        Io10015Template? mailTemplate,
        CancellationToken cancellationToken
    )
    {
        var bookingId = existingReservation.Id.ToString();
        var facilityId = existingReservation.FacilityId.ToString();
        var toMail = existingReservation.Reserver?.EMail;
        var applicationName = MailTemplateSetting.Value.ApplicationName ?? string.Empty;

        if (mailTemplate is null)
        {
            const string errorMsg = $"Mail template {IoType.IO10015} not found";
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
        Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation existingReservation,
        Io10014Template? mailTemplate,
        CancellationToken cancellationToken
    )
    {
        var bookingId = existingReservation.Id.ToString();
        var facilityId = existingReservation.FacilityId.ToString();
        var toMail = existingReservation.Reserver?.EMail;
        var applicationName = MailTemplateSetting.Value.ApplicationName ?? string.Empty;

        if (mailTemplate is null)
        {
            const string errorMsg = $"Mail template {IoType.IO10014} not found";
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
        Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation existingReservation,
        Io10013Template? mailTemplate,
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
            const string errorMsg = $"Mail template {IoType.IO10013} not found";
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
                nameof(BookingNoShowCommandHandler),
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
                    nameof(BookingNoShowCommandHandler),
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
                nameof(BookingNoShowCommandHandler),
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
