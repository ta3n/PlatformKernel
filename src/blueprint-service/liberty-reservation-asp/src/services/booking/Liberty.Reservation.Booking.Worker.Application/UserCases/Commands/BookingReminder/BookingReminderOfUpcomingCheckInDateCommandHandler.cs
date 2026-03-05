using Liberty.Hangfire;
using Liberty.Hangfire.Models;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Templates;
using Liberty.SysIntegrationEvent.Events;
using Newtonsoft.Json;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;
using Liberty.Reservation.Application.Settings;
using Liberty.Reservation.Booking.Worker.Application.Models.Responses;
using Liberty.Reservation.Booking.Worker.Application.Web.ApiService;
using Microsoft.Extensions.Options;

namespace Liberty.Reservation.Booking.Worker.Application.UserCases.Commands.BookingReminder;

public class BookingReminderOfUpcomingCheckInDateCommandHandler(
    ILogger<BookingReminderOfUpcomingCheckInDateCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IFacilityService facilityService,
    IBookingReservationService bookingReservationService,
    IMailTemplateService mailTemplateService,
    IExternalApiService externalApiService,
    IBookingSecureUrlService bookingSecureUrlService,
    IOptions<MailTemplateSetting> mailTemplateSetting
) : ActionCommandHandlerBase<BookingReminderOfUpcomingCheckInDateCommand, BookingReminderOfUpcomingCheckInDateResponse>(unitOfWork, mapper)
{
    protected override async Task<BookingReminderOfUpcomingCheckInDateResponse> HandleAsync(
        BookingReminderOfUpcomingCheckInDateCommand request,
        CancellationToken cancellationToken
    )
    {
        var processed = new List<long>();
        var errors = new List<long>();

        var templateFormatData = await mailTemplateService.FindMailTemplateAsync(
            cancellationToken
        );
        if (templateFormatData is null)
        {
            const string errorMsg = "Mail template not found";
            logger.LogError(errorMsg);
            throw new AppLibertyException(errorMsg);
        }

        var existingFacilities = await facilityService.FindAllAvailableFacilitiesAsync(
            cancellationToken
        );
        var reminderDate = DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset).AddDays(1);

        foreach (var facilityId in existingFacilities.Select(x => x.Id))
        {
            logger.LogInformation(
                "Checking for upcoming check-in reservations for facility {FacilityId} on date {ReminderDate}",
                facilityId,
                reminderDate
            );

            var result = await ProcessUpcomingCheckInFacility(
                facilityId,
                reminderDate,
                templateFormatData,
                cancellationToken
            );
            processed.AddRange(result.processed);
            processed.AddRange(result.errors);
        }

        logger.LogInformation(
            "{HandlerName} - Processed {Count} reservations: {Data}",
            nameof(BookingReminderOfUpcomingCheckInDateCommandHandler),
            processed.Count,
            string.Join(",", processed)
        );

        logger.LogInformation(
            "{HandlerName} - Errors {Count} reservations: {Data}",
            nameof(BookingReminderOfUpcomingCheckInDateCommandHandler),
            errors.Count,
            string.Join(",", errors)
        );

        if (errors.Count > 0)
        {
            throw new AppLibertyException("Some reservations failed to send email");
        }

        return new(
            [.. processed],
            [.. errors]
        );
    }

    private async Task<(long[] processed, long[] errors)> ProcessUpcomingCheckInFacility(
        long facilityId,
        DateTime reminderDate,
        TemplateFormatData templateFormatData,
        CancellationToken cancellationToken
    )
    {
        var pageNumber = 1;
        const int pageSize = 50;

        var processed = new List<long>();
        var errors = new List<long>();
        bool hasNext;
        var reminderDateId = AppDate.GetId(reminderDate);

        do
        {
            var pageBookings = await bookingReservationService.GetPageReminderUpComingCheckInReservationsAsync(
                facilityId,
                reminderDateId,
                Pageable.Of(pageNumber, pageSize),
                reservation => !(
                    reservation.BookingData!.SendMailState.BookingReminderOfUpcomingCheckInDateSend ?? false
                ),
                cancellationToken
            );
            hasNext = pageBookings.HasNext;

            foreach (var booking in pageBookings.Content)
            {
                var languageCode = booking.BookingData!.LanguageCode;
                var isGuest = string.IsNullOrEmpty(booking.UserCode);

                var io10101Template = languageCode switch
                {
                    "en" => templateFormatData.CreateTemplate<Io10101EnTemplate>(IoType.IO10101En),
                    _ => templateFormatData.CreateTemplate<Io10101Template>(IoType.IO10101)
                };
                var io10103Template = languageCode switch
                {
                    "en" => templateFormatData.CreateTemplate<Io10103EnTemplate>(IoType.IO10103En),
                    _ => templateFormatData.CreateTemplate<Io10103Template>(IoType.IO10103)
                };

                logger.LogInformation(
                    "Found upcoming check-in reservation {ReservationId} for facility {FacilityId} on date {ReminderDate}",
                    booking.Id,
                    facilityId,
                    reminderDateId
                );

                var (processedIds, errorIds) = isGuest
                    ? await ProcessUpcomingCheckForGuestAsync(
                        booking,
                        reminderDateId,
                        io10103Template,
                        cancellationToken
                    )
                    : await ProcessUpcomingCheckForUserAsync(
                        booking,
                        reminderDateId,
                        io10101Template,
                        cancellationToken
                    );

                processed.AddRange(processedIds);
                errors.AddRange(errorIds);
            }

            if (!pageBookings.HasNext)
            {
                break;
            }

            pageNumber++;
        } while (hasNext);

        return ([.. processed], [.. errors]);
    }

    private async Task<(List<long> processed, List<long> errors)> ProcessUpcomingCheckForUserAsync(
        ReservationEntity booking,
        long reminderDateId,
        Io10101Template? template,
        CancellationToken cancellationToken
    )
    {
        var applicationName = mailTemplateSetting.Value.ApplicationName ?? string.Empty;
        var processed = new List<long>();
        var errors = new List<long>();

        if (template is null)
        {
            errors.Add(booking.Id);

            return (processed, errors);
        }

        var bookingId = booking.Id;
        var facilityId = booking.FacilityId;
        var toMail = booking.Reserver?.EMail;

        var jpDateNow = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(9));
        var jpMidnight = new DateTimeOffset(jpDateNow.Year, jpDateNow.Month, jpDateNow.Day, 0, 0, 0, jpDateNow.Offset);

        if (string.IsNullOrEmpty(toMail))
        {
            logger.LogWarning(
                "No email address found for reservation {ReservationId} in facility {FacilityId} on date {ReminderDate}",
                bookingId,
                facilityId,
                reminderDateId
            );
            errors.Add(bookingId);
        }
        else
        {
            template.URL = mailTemplateSetting.Value.UserUrl;
            template.SetData(booking, applicationName);

            var message = new
            {
                bookingId,
                tos = new[] { toMail },
                subject = template.Subject,
                body = template.Body
            };

            var isRegistered = await RegisterScheduleJobAsync(
                booking.Id,
                new RegisterScheduleJobAtRequest(
                    nameof(BookingReminderOfUpcomingCheckInDateSendMailEvent),
                    $"{nameof(BookingReminderOfUpcomingCheckInDateSendMailEvent)}_Facility_{facilityId}_Reservation_{booking.Id}",
                    JsonConvert.SerializeObject(message),
                    jpMidnight
                ),
                cancellationToken
            );
            if (isRegistered)
            {
                processed.Add(bookingId);
            }
            else
            {
                errors.Add(bookingId);
            }
        }

        return (processed, errors);
    }

    private async Task<(List<long> processed, List<long> errors)> ProcessUpcomingCheckForGuestAsync(
        ReservationEntity booking,
        long reminderDateId,
        Io10103Template? template,
        CancellationToken cancellationToken
    )
    {
        var applicationName = mailTemplateSetting.Value.ApplicationName ?? string.Empty;
        var processed = new List<long>();
        var errors = new List<long>();

        if (template is null)
        {
            errors.Add(booking.Id);

            return (processed, errors);
        }

        var bookingId = booking.Id;
        var facilityId = booking.FacilityId;
        var toMail = booking.Reserver?.EMail;

        var jpDateNow = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(9));
        var jpMidnight = new DateTimeOffset(jpDateNow.Year, jpDateNow.Month, jpDateNow.Day, 0, 0, 0, jpDateNow.Offset);

        if (string.IsNullOrEmpty(toMail))
        {
            logger.LogWarning(
                "No email address found for reservation {ReservationId} in facility {FacilityId} on date {ReminderDate}",
                bookingId,
                facilityId,
                reminderDateId
            );
            errors.Add(bookingId);
        }
        else
        {
            var validMinutes = (int)(AppDate.GetDateTime(booking.CheckInDate, booking.CheckInTime) - DateTime.UtcNow)
                .TotalMinutes;
            var secureRequest = new BookingSecureUrlRequest(
                $"{booking.Id}",
                validMinutes
            );
            var guestCode = bookingSecureUrlService.EncryptDataWithHmacSha256(secureRequest);
            template.URL = mailTemplateSetting.Value.GuestUrl?.Replace("{guestCode}", guestCode) ?? string.Empty;
            template.SetData(booking, applicationName);

            var message = new
            {
                bookingId,
                tos = new[] { toMail },
                subject = template.Subject,
                body = template.Body
            };

            var isRegistered = await RegisterScheduleJobAsync(
                booking.Id,
                new RegisterScheduleJobAtRequest(
                    nameof(BookingReminderOfUpcomingCheckInDateSendMailEvent),
                    $"{nameof(BookingReminderOfUpcomingCheckInDateSendMailEvent)}_Facility_{facilityId}_Reservation_{booking.Id}",
                    JsonConvert.SerializeObject(message),
                    jpMidnight
                ),
                cancellationToken
            );
            if (isRegistered)
            {
                processed.Add(bookingId);
            }
            else
            {
                errors.Add(bookingId);
            }
        }

        return (processed, errors);
    }

    private async Task<bool> RegisterScheduleJobAsync(
        long reservationId,
        RegisterScheduleJobAtRequest request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var (_, context) = await externalApiService.PostAsync(
                ExternalService.BatchSchedulerService,
                RegisterJobEndpoint.RegisterScheduleJobAtEndpoint,
                JsonConvert.SerializeObject(request),
                cancellationToken
            );

            var isRegistered = !string.IsNullOrEmpty(context);
            if (isRegistered)
            {
                await bookingReservationService.UpdateBookingSendMailStateAsync(
                    reservationId,
                    BookingSendMailState.ReminderOfUpcomingCheckInDateSendMail,
                    cancellationToken
                );
            }

            return isRegistered;
        }
        catch
        {
            return false;
        }
    }
}
