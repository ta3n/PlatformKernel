using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

public record ReservationGetAllOptionItemsQuery(
    long Id,
    int AppDateId,
    int RoomGroupIndex,
    IPageable Pageable
) : IQueryPagedBase<OptionItemOfPlanResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
