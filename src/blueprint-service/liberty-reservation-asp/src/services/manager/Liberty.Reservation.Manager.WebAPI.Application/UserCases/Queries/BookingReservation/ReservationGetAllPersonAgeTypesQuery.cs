using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

public record ReservationGetAllPersonAgeTypesQuery(
    long Id,
    IPageable Pageable
) : IQueryPagedBase<PersonAgeTypeResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
