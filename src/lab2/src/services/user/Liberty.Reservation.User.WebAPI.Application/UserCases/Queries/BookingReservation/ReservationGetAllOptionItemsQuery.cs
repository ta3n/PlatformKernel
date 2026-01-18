using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;

public record ReservationGetAllOptionItemsQuery(
    long Id,
    int AppDateId,
    int RoomGroupIndex,
    IPageable Pageable
) : IQueryPagedBase<OptionItemOfPlanResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
