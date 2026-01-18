using Liberty.Pagination;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingAggregateFaxAuditLog;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/booking-aggregate-fax-audit-log")]
public class BookingAggregateFaxAuditLogEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<BookingAggregateFaxAuditLogSearchResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchBookingAggregateFaxAuditLog(
        IPageable pageable,
        CancellationToken cancellationToken = default
    )
    {
        var (headers, response) = await Mediator!.Send(
            new SearchBookingAggregateFaxAuditLogQuery(pageable),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
