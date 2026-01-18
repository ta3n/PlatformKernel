using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Sale;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/sales")]
public class SalesEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(List<SaleDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSaleDetails(
        IPageable pageable,
        [FromQuery] long startAppDateId,
        [FromQuery] long endAppDateId,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new SaleGetAllReservationsQuery(
                startAppDateId,
                endAppDateId,
                pageable
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("overview")]
    [ProducesResponseType(typeof(SaleOverviewResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSalesOverview(
        [FromQuery] long startAppDateId,
        [FromQuery] long endAppDateId,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new SaleGetReservationOverviewQuery(
                startAppDateId,
                endAppDateId
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
