namespace Liberty.Reservation.Site.Application.Domains.Services.Interfaces;

public interface IPlanService
{
    Task<(bool IsAvailable, PlanTypes PlanType)> CheckPlanAlreadyAsync(
        long facilityId,
        long planId,
        CancellationToken cancellationToken
    );

    Task<Plan?> GetPlanDetailCancellation(
        long planId,
        CancellationToken cancellationToken
    );

    Task<bool> CheckRoomAlreadyInPlanAsync(
        long planId,
        long roomGroupId,
        CancellationToken cancellationToken
    );

    Task<bool> CheckPaymentOnlinePaymentAvailableAsync(
        long facilityId,
        long planId,
        CancellationToken cancellationToken
    );

    Task<Plan?> GetPlanAtCreateBookingAsync(
        long planId,
        CancellationToken cancellationToken
    );

    Task<bool> IsValidCheckInTimeAsync(
        long planId,
        long checkInDate,
        CancellationToken cancellationToken
    );

    Task<bool> CheckPaymentOnSitePaymentAvailableAsync(
        long facilityId,
        long planId,
        CancellationToken cancellationToken
    );

    Task<TimeSpan?> GetCheckOutTimeAsync(
        long planId,
        CancellationToken cancellationToken
    );
}
