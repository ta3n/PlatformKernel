using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

public record ReservationGetAllQuery(
    long StartAppDateId,
    long EndAppDateId,
    IPageable Pageable
) : IQueryPagedBase<BookingReservationResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
