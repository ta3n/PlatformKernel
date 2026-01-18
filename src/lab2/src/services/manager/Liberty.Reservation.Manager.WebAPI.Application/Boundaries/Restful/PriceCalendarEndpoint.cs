using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PriceCalendar;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PriceCalendar;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/price-calendar")]
public class PriceCalendarEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> AdjustPriceOfCalendar(
        [FromBody] PriceCalendarCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        await Mediator!.Send(
            new PriceCalendarCreateCommand { Payload = request },
            cancellationToken
        );

        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<PriceCalendarResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPricesOfCalendar(
        IPageable pageable,
        [FromQuery] int startDate,
        [FromQuery] int endDate,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PriceCalendarGetAllQuery(
                startDate,
                endDate,
                pageable
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
