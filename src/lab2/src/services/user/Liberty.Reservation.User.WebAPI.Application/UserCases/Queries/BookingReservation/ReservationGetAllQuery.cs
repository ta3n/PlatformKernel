using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;

public record ReservationGetAllQuery(
    IPageable Pageable
) : IQueryPagedBase<ReservationResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
