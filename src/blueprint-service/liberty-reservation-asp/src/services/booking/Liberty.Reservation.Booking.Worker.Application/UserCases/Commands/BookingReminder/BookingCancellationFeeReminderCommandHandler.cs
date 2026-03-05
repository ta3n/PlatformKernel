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

public class BookingCancellationFeeReminderCommandHandler(
    ILogger<BookingCancellationFeeReminderCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IFacilityService facilityService,
    IBookingReservationService bookingReservationService,
    IMailTemplateService mailTemplateService,
    IExternalApiService externalApiService,
    IBookingSecureUrlService bookingSecureUrlService,
    IOptions<MailTemplateSetting> mailTemplateSetting
) : ActionCommandHandlerBase<BookingCancellationFeeReminderCommand, BooingCancellationFeeReminderResponse>(unitOfWork, mapper)
{
    protected override async Task<BooingCancellationFeeReminderResponse> HandleAsync(
        BookingCancellationFeeReminderCommand request,
        CancellationToken cancellationToken
    )
    {
        var processed = new List<long>();
        var errors = new List<long>();

        var templateFormatData = await GetTemplateFormatDataAsync(cancellationToken);

        var existingFacilities = await facilityService.FindAllAvailableFacilitiesAsync(
            cancellationToken
        );
        var reminderDate = DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset).AddDays(1);

        foreach (var facility in existingFacilities)
        {
            await ProcessFacilityAsync(facility, reminderDate, templateFormatData, processed, errors, cancellationToken);
        }

        logger.LogInformation(
            "{HandlerName} - Processed {Count} reservations: {Data}",
            nameof(BookingCancellationFeeReminderCommandHandler),
            processed.Count,
            string.Join(",", processed)
        );

        logger.LogInformation(
            "{HandlerName} - Errors {Count} reservations: {Data}",
            nameof(BookingCancellationFeeReminderCommandHandler),
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

    private async Task<TemplateFormatData> GetTemplateFormatDataAsync(
        CancellationToken cancellationToken
    )
    {
        var templateFormatData = await mailTemplateService.FindMailTemplateAsync(cancellationToken);
        if (templateFormatData is not null)
        {
            return templateFormatData;
        }

        const string errorMsg = "Mail template not found";
        logger.LogError(errorMsg);
        throw new AppLibertyException(errorMsg);
    }

    private async Task ProcessFacilityAsync(
        Facility facility,
        DateTime reminderDate,
        TemplateFormatData templateFormatData,
        List<long> processed,
        List<long> errors,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "Checking for cancellation fee reservations for facility {FacilityId} on date {ReminderDate}",
            facility.Id,
            reminderDate
        );

        var plans = facility.FacilityPlans?
            .Select(x => x.Plan)
            .Where(plan => plan is { IsEnabled: true, IsDeleted: false })
            .Distinct()
            .ToList();

        if (plans is null or { Count: 0 })
        {
            return;
        }

        foreach (var plan in plans.OfType<Plan>())
        {
            await ProcessPlanAsync(facility.Id, plan, reminderDate, templateFormatData, processed, errors, cancellationToken);
        }
    }

    private async Task ProcessPlanAsync(
        long facilityId,
        Plan plan,
        DateTime reminderDate,
        TemplateFormatData templateFormatData,
        List<long> processed,
        List<long> errors,
        CancellationToken cancellationToken
    )
    {
        var result = await ProcessCancellationFeeInFacility(
            facilityId,
            plan.Id,
            reminderDate,
            templateFormatData,
            cancellationToken
        );

        processed.AddRange(result.processed);
        errors.AddRange(result.errors);
    }

    private async Task<(long[] processed, long[] errors)> ProcessCancellationFeeInFacility(
        long facilityId,
        long planId,
        DateTime reminderDate,
        TemplateFormatData? templateFormatData,
        CancellationToken cancellationToken
    )
    {
        var pageNumber = 1;
        const int pageSize = 50;

        var processed = new List<long>();
        var errors = new List<long>();
        bool hasNext;

        do
        {
            var pageBookings = await bookingReservationService.GetPageReminderCancelCheckInReservationsAsync(
                facilityId,
                reminderDate,
                Pageable.Of(pageNumber, pageSize),
                reservation => !(
                        reservation.BookingData!.SendMailState.BookingCancellationFeeReminderSend ?? false
                    )
                    && reservation.PlanId == planId,
                cancellationToken
            );
            hasNext = pageBookings.HasNext;

            foreach (var booking in pageBookings.Content)
            {
                var reminderDateId = bookingReservationService.CalculateReminderDateId(booking.BookingData, reminderDate);
                logger.LogInformation(
                    "Found cancellation fee reservation {ReservationId} for facility {FacilityId} on date {ReminderDate}",
                    booking.Id,
                    facilityId,
                    reminderDateId
                );

                var languageCode = booking.BookingData!.LanguageCode;
                var isGuest = string.IsNullOrEmpty(booking.UserCode);

                var io10102Template = languageCode switch
                {
                    "en" => templateFormatData!.CreateTemplate<Io10102EnTemplate>(IoType.IO10102En),
                    _ => templateFormatData!.CreateTemplate<Io10102Template>(IoType.IO10102)
                };
                var io10104Template = languageCode switch
                {
                    "en" => templateFormatData.CreateTemplate<Io10104EnTemplate>(IoType.IO10104En),
                    _ => templateFormatData.CreateTemplate<Io10104Template>(IoType.IO10104)
                };

                var (processedIds, errorIds) = isGuest
                    ? await ProcessBookingCancellationFeeForGuestAsync(
                        booking,
                        reminderDateId,
                        io10104Template,
                        cancellationToken
                    )
                    : await ProcessBookingCancellationFeeForUserAsync(
                        booking,
                        reminderDateId,
                        io10102Template,
                        cancellationToken
                    );
                processed.AddRange(processedIds);
                errors.AddRange(errorIds);
            }

            pageNumber++;
        } while (hasNext);

        return ([.. processed], [.. errors]);
    }

    private async Task<(List<long> processed, List<long> errors)> ProcessBookingCancellationFeeForUserAsync(
        ReservationEntity booking,
        long reminderDateId,
        Io10102Template? template,
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

        var jpDateNow = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(DefaultValues.TimeZoneOffset));
        var jpMidnight = new DateTimeOffset(
            jpDateNow.Year,
            jpDateNow.Month,
            jpDateNow.Day,
            0,
            0,
            0,
            jpDateNow.Offset
        );

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
                bookingId,
                new RegisterScheduleJobAtRequest(
                    nameof(BookingCancellationFeeReminderSendMailEvent),
                    $"{nameof(BookingCancellationFeeReminderSendMailEvent)}_Facility_{facilityId}_Reservation_{bookingId}",
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

    private async Task<(List<long> processed, List<long> errors)> ProcessBookingCancellationFeeForGuestAsync(
        ReservationEntity booking,
        long reminderDateId,
        Io10104Template? template,
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

        var jpDateNow = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(DefaultValues.TimeZoneOffset));
        var jpMidnight = new DateTimeOffset(
            jpDateNow.Year,
            jpDateNow.Month,
            jpDateNow.Day,
            0,
            0,
            0,
            jpDateNow.Offset
        );

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
                bookingId,
                new RegisterScheduleJobAtRequest(
                    nameof(BookingCancellationFeeReminderSendMailEvent),
                    $"{nameof(BookingCancellationFeeReminderSendMailEvent)}_Facility_{facilityId}_Reservation_{bookingId}",
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
                    BookingSendMailState.CancellationFeeReminderSendMail,
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
