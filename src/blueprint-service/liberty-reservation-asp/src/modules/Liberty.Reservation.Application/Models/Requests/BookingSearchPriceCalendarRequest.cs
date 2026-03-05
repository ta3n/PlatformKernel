namespace Liberty.Reservation.Application.Models.Requests;

public record BookingSearchPriceCalendarRequest(
    IEnumerable<PlanRoomOfBookingSearchPriceCalendarRequest> PlanRoomGroups,
    BookingSearchPlanRequest BookingSearch
)
{
    public IEnumerable<long> GetPlanIds()
    {
        return PlanRoomGroups.Select(x => x.PlanId).Distinct();
    }

    public IEnumerable<long> GetRoomIds()
    {
        return PlanRoomGroups.Select(x => x.RoomGroupId).Distinct();
    }
};

public record PlanRoomOfBookingSearchPriceCalendarRequest(
    long PlanId,
    long RoomGroupId
);
