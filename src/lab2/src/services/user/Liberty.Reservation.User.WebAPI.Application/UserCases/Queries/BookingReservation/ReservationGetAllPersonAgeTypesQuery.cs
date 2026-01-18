using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;

public record ReservationGetAllPersonAgeTypesQuery(
    long Id,
    IPageable Pageable
) : IQueryPagedBase<PersonAgeTypeResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
