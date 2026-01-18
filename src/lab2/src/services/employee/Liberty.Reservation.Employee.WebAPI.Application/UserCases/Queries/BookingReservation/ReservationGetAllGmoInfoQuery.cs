using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingReservation;

public record ReservationGetAllGmoInfoQuery(
    IPageable Pageable
) : IQueryPagedBase<GmoPaymentResultResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
