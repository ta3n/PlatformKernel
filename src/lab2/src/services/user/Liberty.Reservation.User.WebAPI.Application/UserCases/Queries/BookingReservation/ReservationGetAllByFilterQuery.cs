using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.User.WebAPI.Application.Models.Requests;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;

public record ReservationGetAllByFilterQuery(
    IPageable Pageable,
    BookingFilterRequest bookingFilterRequest
) : IQueryPagedBase<ReservationResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
