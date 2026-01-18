using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingAggregateFaxAuditLog;

public record SearchBookingAggregateFaxAuditLogQuery(
    IPageable Pageable
) : IQueryPagedBase<BookingAggregateFaxAuditLogSearchResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
