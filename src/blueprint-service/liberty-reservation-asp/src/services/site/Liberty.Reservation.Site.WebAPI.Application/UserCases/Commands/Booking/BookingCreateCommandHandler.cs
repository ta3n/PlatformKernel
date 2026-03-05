using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Site.WebAPI.Application.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking;

public class BookingCreateCommandHandler(
    ILogger<BookingCreateCommandHandler> logger,
    IMediator mediator,
    IPlanService planService,
    IQuestionService questionService,
    ISecurityContextAccessor securityContextAccessor,
    IServiceProvider serviceProvider
) : CreateCommandHandlerBase<BookingCreateCommand, string>(null!, null!)
{
    private readonly IBookingHoldManagementService _bookingHoldManagementService
        = serviceProvider.GetRequiredService<IBookingHoldManagementService>();

    private readonly BookingCreateSendEmailBackgroundService _bookingConfirmSendEmailBackgroundService
        = serviceProvider.GetRequiredService<BookingCreateSendEmailBackgroundService>();

    protected override async Task<string> HandleAsync(
        BookingCreateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var facilityId = securityContextAccessor.GetFacilityIdSelected();
        var facilityCode = securityContextAccessor.GetFacilityCodeSelected();
        var siteIdOfFacility = securityContextAccessor.GetSiteIdSelected();
        var siteCodeOfFacility = securityContextAccessor.GetSiteCodeSelected();
        var userKey = securityContextAccessor.GetApplicationUserKey();
        var languageCode = securityContextAccessor.GetLanguageCode();
        var facilityRecordCode = securityContextAccessor.FacilityRecordCode;
        var isUserUseOnlinePayment = !string.IsNullOrEmpty(userKey) && payload.PaymentType is PaymentTypes.OnLinePayment;

        // Check the validity of the plan and the conditions
        var (_, planType) = await ValidatePlanAndConditionsAsync(
            request,
            facilityId,
            userKey,
            cancellationToken
        );

        // Processing questions
        var existingQuestions = await ProcessQuestionsAsync(
            payload,
            cancellationToken
        );

        var checkOutTime = await planService.GetCheckOutTimeAsync(
            request.PlanId,
            cancellationToken
        );

        // Create reservation requirements
        var bookingCreateReq = new BookingCreateRequest(
            securityContextAccessor.GetLanguageCode(),
            facilityId,
            siteIdOfFacility,
            request.PlanId,
            request.RoomGroupId,
            payload.CheckInDate,
            payload.CheckInTime,
            checkOutTime,
            payload.PaymentType,
            payload.Adjust,
            facilityRecordCode,
            payload.PlanQuestions,
            payload.OptionsQuestions
        )
        {
            PlanType = planType,
            TimeZoneOffset = securityContextAccessor.TimeZoneOffset
        };

        // Create a room holder
        var bookingHoldCheckModel = new BookingHoldCheckModel(
            facilityId,
            siteIdOfFacility,
            request.PlanId,
            request.RoomGroupId,
            payload.CheckInDate,
            payload.Adjust.NumberOfNights,
            payload.Adjust.NumberOfRooms,
            userKey,
            bookingCreateReq.TempCode,
            isUserUseOnlinePayment
                ? BookingHoldValues.HoldTimeInSecondsForUserOnlinePayment
                : BookingHoldValues.DefaultHoldTimeInSeconds
        );

        try
        {
            // Try to keep the room
            var isRoomHeld = await _bookingHoldManagementService.TryHoldRoomAsync(
                bookingHoldCheckModel,
                cancellationToken
            );
            if (!isRoomHeld)
            {
                throw new ReservationNoRemainRoomNumberRestException();
            }

            // Create a reservation
            var bookingExternalInfoReq = new BookingExternalInfoRequest(
                userKey,
                existingQuestions
            );
            var newReservation = await mediator.Send(
                new Reservation.Application.UseCases.Commands.BookingReservation.BookingCreateCommand(
                    bookingExternalInfoReq
                ) { Payload = bookingCreateReq },
                cancellationToken
            );

            // Liberation room if successfully booked
            if (newReservation.ReservationState is ReservationStatus.Reserved or ReservationStatus.Temporary)
            {
                await _bookingHoldManagementService.ReleaseHoldAsync(
                    bookingHoldCheckModel,
                    cancellationToken
                );
            }

            // Handling after successful booking
            _ = Task.Run(
                async () =>
                {
                    try
                    {
                        await _bookingConfirmSendEmailBackgroundService.EnqueueEmailJobAsync(
                            new BookingCreateSendEmailJob(
                                facilityCode,
                                siteCodeOfFacility,
                                languageCode,
                                newReservation
                            ),
                            CancellationToken.None
                        );
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(
                            ex,
                            "Failed to enqueue email job for booking {BookingId}",
                            newReservation.Id
                        );
                    }
                },
                CancellationToken.None
            );

            return newReservation.Code ?? string.Empty;
        }
        catch
        {
            await _bookingHoldManagementService.ReleaseHoldAsync(
                bookingHoldCheckModel,
                cancellationToken
            );
            throw;
        }
    }

    private async Task<(bool isAvailable, PlanTypes planType)> ValidatePlanAndConditionsAsync(
        BookingCreateCommand request,
        long facilityId,
        string? userKey,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        // Check online payment for guests
        if (userKey is null && payload.PaymentType is PaymentTypes.OnLinePayment)
        {
            throw new OnlinePaymentNotAllowedForGuestException();
        }

        // Check the plan
        var (isAvailable, planType) = await planService.CheckPlanAlreadyAsync(
            facilityId,
            request.PlanId,
            cancellationToken
        );

        if (!isAvailable)
        {
            throw new PlanNotfoundException();
        }

        // Check check-in time
        var isValidCheckInTime = await planService.IsValidCheckInTimeAsync(
            request.PlanId,
            payload.CheckInDate,
            cancellationToken
        );
        if (!isValidCheckInTime)
        {
            throw new ReservationInvalidException("Invalid check-in time");
        }

        // Online payment check
        var existingPaymentOnline = await planService.CheckPaymentOnlinePaymentAvailableAsync(
            facilityId,
            request.PlanId,
            cancellationToken
        );
        if (payload.PaymentType is PaymentTypes.OnLinePayment && !existingPaymentOnline)
        {
            throw new PlanPaymentOnlineNotAvailableException();
        }

        // Onsite payment check
        var existingAllowOnSite = await planService.CheckPaymentOnSitePaymentAvailableAsync(
            facilityId,
            request.PlanId,
            cancellationToken
        );
        if (payload.PaymentType is PaymentTypes.OnSidePayment && !existingAllowOnSite)
        {
            throw new PlanPaymentOnSiteNotAvailableException();
        }

        // Check the room group in the plan
        var existingRoomGroupInPlan = await planService.CheckRoomAlreadyInPlanAsync(
            request.PlanId,
            request.RoomGroupId,
            cancellationToken
        );
        if (!existingRoomGroupInPlan)
        {
            throw new RoomGroupNotAlreadyInPlanException();
        }

        return (isAvailable, planType);
    }

    private async Task<List<Question>> ProcessQuestionsAsync(
        SiteBookingCreateRequest payload,
        CancellationToken cancellationToken
    )
    {
        var questionIds = new List<long>();
        if (payload.PlanQuestions is not null)
        {
            questionIds.AddRange(payload.PlanQuestions!.Select(x => x.QuestionId));
        }

        if (payload.OptionsQuestions is not null)
        {
            questionIds.AddRange(payload.OptionsQuestions!.Select(x => x.QuestionId));
        }

        List<Question> existingQuestions = [];
        if (questionIds.Count <= 0)
        {
            return existingQuestions;
        }

        var distinctQuestionIds = questionIds.Distinct().ToList();
        existingQuestions =
        [
            .. await questionService.FindAllByIdsAsync(
                [.. distinctQuestionIds],
                cancellationToken
            )
        ];
        var existingQuestionsCount = existingQuestions.Count;

        if (existingQuestionsCount != distinctQuestionIds.Count)
        {
            throw new QuestionNotfoundException();
        }

        return existingQuestions;
    }
}
