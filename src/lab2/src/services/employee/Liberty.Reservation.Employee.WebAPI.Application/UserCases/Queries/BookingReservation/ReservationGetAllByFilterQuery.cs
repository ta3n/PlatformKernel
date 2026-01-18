using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingReservation;

public record ReservationGetAllByFilterQuery(
    ReservationGetAllBylFilterRequest Request,
    IPageable Pageable
) : IQueryPagedBase<BookingReservationResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
