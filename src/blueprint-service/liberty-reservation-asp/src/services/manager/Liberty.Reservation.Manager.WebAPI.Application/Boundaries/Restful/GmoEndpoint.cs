using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.UseCases.Queries.BookingReservation;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/gmo")]
public class GmoEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet("search/{id:long:min(1)}")]
    [ProducesResponseType(typeof(SearchTradeResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchTrade(
        [FromRoute] long id,
        CancellationToken cancellationToken = default
    )
    {
        var (headers, response) = await Mediator!.Send(
            new GmoSearchTradeQuery(id),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
