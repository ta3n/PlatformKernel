using Liberty.Fax.Services.Interfaces;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingAggregateFaxAuditLog;

public class SearchBookingAggregateFaxAuditLogQueryHandler(
    IMapper mapper,
    IBookingAggregateFaxAuditLogRepository faxAuditLogRepository,
    IFaxErrorService faxErrorService
) : QueryPageBaseHandler<SearchBookingAggregateFaxAuditLogQuery, BookingAggregateFaxAuditLogSearchResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<BookingAggregateFaxAuditLogSearchResponse>)> HandleAsync(
        SearchBookingAggregateFaxAuditLogQuery request,
        CancellationToken cancellationToken
    )
    {
        var pageable = request.Pageable;

        var errorMap = faxErrorService
            .GetAll()
            .ToDictionary(
                x => x.Code,
                x => x.Message
            );

        var query = faxAuditLogRepository.GetQueryableWithAsNoTracking();

        var selectQuery = query
            .OrderByDescending(x => x.CreatedAt)
            .Select(
                x => new BookingAggregateFaxAuditLogSearchResponse(
                    x.Id,
                    x.FaxNumber,
                    x.Subject,
                    x.Result,
                    x.IsSuccess,
                    x.AggregateCode,
                    x.Body
                ) { CreatedAtId = x.CreatedAt }
            );

        var page = await selectQuery.UsePageableAsync(pageable, cancellationToken: cancellationToken);
        var content = page.Content.ToList();
        foreach (var item in content)
        {
            item.ResultMessage = errorMap.GetValueOrDefault(item.Result ?? string.Empty) ?? string.Empty;
        }

        var headers = page.GeneratePaginationHttpHeaders();

        return (headers, content);
    }
}
