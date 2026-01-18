using Liberty.Pagination;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Sales;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/sales")]
public class SaleEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpPost("overview")]
    [ProducesResponseType(typeof(SaleOverviewResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSalesOverview(
        [FromBody] SaleGetAllRequest request,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new SaleGetReservationOverviewQuery(
                request
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPost]
    [ProducesResponseType(typeof(List<SaleDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSaleDetails(
        [FromQuery] IPageable pageable,
        [FromBody] SaleGetAllRequest request,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new SaleGetAllReservationsQuery(
                request,
                pageable
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPost("export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportSaleCsv(
        [FromBody] SaleGetAllRequest request,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new SaleExportCsvQuery(
                request
            ),
            cancellationToken
        );

        foreach (var header in headers)
        {
            Response.Headers[header.Key] = header.Value;
        }

        await response.WriteToStreamAsync(Response.Body, cancellationToken);

        return new EmptyResult();
    }
}
