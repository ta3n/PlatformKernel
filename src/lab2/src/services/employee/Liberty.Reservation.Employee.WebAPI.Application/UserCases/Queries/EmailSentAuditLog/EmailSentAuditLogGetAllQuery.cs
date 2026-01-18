using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.EmailSentAuditLog;

public record EmailSentAuditLogGetAllQuery(
    IPageable Pageable
) : IQueryPagedBase<EmailSentAuditLogResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
