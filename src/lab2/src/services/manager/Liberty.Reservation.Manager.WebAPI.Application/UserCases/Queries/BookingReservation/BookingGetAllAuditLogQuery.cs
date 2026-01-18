using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

public record BookingGetAllAuditLogQuery(
    long Id,
    IPageable Pageable
) : IQueryPagedBase<BookingAuditLogResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
