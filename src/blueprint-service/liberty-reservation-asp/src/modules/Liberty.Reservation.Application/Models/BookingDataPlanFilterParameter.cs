using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Application.Models;

public record BookingDataPlanFilterParameter(
    long FacilityId,
    long SiteId,
    long[]? PlanIds,
    long[]? RoomGroupIds,
    bool IsSecret,
    bool IsLoadingPriceAppDate,
    BookingSearchPlanRequest Search
)
{
    public bool SystemCanOnlinePayment { get; init; }
    public FacilityStateModel FacilityState { get; init; } = null!;
}
