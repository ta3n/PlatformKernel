using AutoMapper.QueryableExtensions;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.EmailSentAuditLog;

public class EmailSentAuditLogGetAllQueryHandler(
    IMapper mapper,
    IBookingAggregateMailAuditLogRepository emailSentAuditLogRepository
) : QueryPageBaseHandler<EmailSentAuditLogGetAllQuery, EmailSentAuditLogResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<EmailSentAuditLogResponse>)> HandleAsync(
        EmailSentAuditLogGetAllQuery query,
        CancellationToken cancellationToken
    )
    {
        var queryable = emailSentAuditLogRepository
            .GetQueryableWithAsNoTracking()
            .ProjectTo<EmailSentAuditLogResponse>(Mapper.ConfigurationProvider);

        var page = await queryable.UsePageableAsync(
            query.Pageable,
            false,
            cancellationToken
        );

        var headers = page.GeneratePaginationHttpHeaders();
        var data = page.Content;

        return (headers, data);
    }
}
