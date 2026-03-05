using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.UseCases.Queries.BookingReservation;

public record BookingGetAllAuditLogQuery(
    long BookingId,
    IPageable Pageable
) : IQueryPagedBase<BookingAuditLogResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
