using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupPrice;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupPrice;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/room-group-prices/{id:long:min(1)}/destinations/{siteId:long:min(1)}")]
public class RoomGroupPricesEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateRoomTypeSiteAsync(
        [FromRoute] long id,
        [FromRoute] long siteId,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new RoomGroupPriceCreateSiteCommand
            {
                Payload = new RoomGroupSiteCreateRequest(
                    -1,
                    id,
                    siteId
                )
            },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response)
            .WithHeaders(
                HeaderUtil.CreateEntityCreationAlert(
                    "PlanRoomSite",
                    response.ToString()
                )
            );
    }

    [HttpPatch("standard-price")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateStandardPriceOfRoomGroupPrice(
        [FromRoute] long id,
        [FromRoute] long siteId,
        [FromBody] RoomGroupPriceUpdateStandardPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        await Mediator!.Send(
            new RoomGroupPriceUpdateStandardPriceCommand(
                id,
                siteId
            ) { Payload = request },
            cancellationToken
        );

        return NoContent();
    }

    [HttpPatch("children-price")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateChildrenPriceOfRoomGroupPrice(
        [FromRoute] long id,
        [FromRoute] long siteId,
        [FromBody] RoomGroupPriceUpdateChildrenPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        await Mediator!.Send(
            new RoomGroupPriceUpdateChildrenPriceCommand(
                id,
                siteId
            ) { Payload = request },
            cancellationToken
        );

        return NoContent();
    }

    [HttpPatch("sale")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateSaleOfRoomGroupPrice(
        [FromRoute] long id,
        [FromRoute] long siteId,
        [FromBody] RoomGroupPriceUpdateSaleRequest request,
        CancellationToken cancellationToken
    )
    {
        await Mediator!.Send(
            new RoomGroupPriceUpdateSaleCommand(
                id,
                siteId
            ) { Payload = request },
            cancellationToken
        );

        return NoContent();
    }

    [HttpPatch("discount")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateDiscountOfRoomGroupPrice(
        [FromRoute] long id,
        [FromRoute] long siteId,
        [FromBody] RoomGroupPriceUpdateDiscountRequest request,
        CancellationToken cancellationToken
    )
    {
        await Mediator!.Send(
            new RoomGroupPriceUpdateDiscountCommand(
                id,
                siteId
            ) { Payload = request },
            cancellationToken
        );

        return NoContent();
    }

    [HttpPatch("price-calendar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdatePriceCalendarOfRoomGroupPrice(
        [FromRoute] long id,
        [FromRoute] long siteId,
        [FromBody] RoomGroupPriceUpdatePriceCalendarRequest request,
        CancellationToken cancellationToken
    )
    {
        await Mediator!.Send(
            new RoomGroupPriceUpdatePriceCalendarCommand(
                id,
                siteId
            ) { Payload = request },
            cancellationToken
        );

        return NoContent();
    }

    [HttpGet("standard-price")]
    [ProducesResponseType(typeof(PlanRoomDetailStandardPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStandardPriceOfRoomGroupPrice(
        [FromRoute] long id,
        [FromRoute] long siteId,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new RoomGroupPriceGetStandardPriceQuery(
                id,
                siteId
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response)
            .WithHeaders(headers);
    }

    [HttpGet("children-price")]
    [ProducesResponseType(typeof(PlanRoomDetailChildrenPriceResponse), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetChildrenPriceOfRoomGroupPrice(
        [FromRoute] long id,
        [FromRoute] long siteId,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new RoomGroupPriceGetChildrenPriceQuery(
                id,
                siteId
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("sale")]
    [ProducesResponseType(typeof(PlanRoomDetailSaleResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSaleOfRoomGroupPrice(
        [FromRoute] long id,
        [FromRoute] long siteId,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new RoomGroupPriceGetSaleQuery(
                id,
                siteId
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("discount")]
    [ProducesResponseType(typeof(PlanRoomDetailDiscountResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDiscountOfRoomGroupPrice(
        [FromRoute] long id,
        [FromRoute] long siteId,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new RoomGroupPriceGetDiscountQuery(
                id,
                siteId
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("price-calendar")]
    [ProducesResponseType(typeof(PlanRoomDetailPriceCalendarResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPriceCalendarOfRoomGroupPrice(
        [FromRoute] long id,
        [FromRoute] long siteId,
        [FromQuery] long startDate,
        [FromQuery] long endDate,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new RoomGroupPriceGetPriceCalendarQuery(
                id,
                siteId,
                startDate,
                endDate
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
