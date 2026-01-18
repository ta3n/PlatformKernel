using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/plan-prices/{id:long:min(1)}/room-groups/{roomTypeId:long:min(1)}/destinations/{siteId:long:min(1)}")]
public class PlanPricesEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateRoomTypeSiteAsync(
        [FromRoute] long id,
        [FromRoute] long roomTypeId,
        [FromRoute] long siteId,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanPriceCreateSiteCommand
            {
                Payload = new RoomGroupSiteCreateRequest(
                    id,
                    roomTypeId,
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateRoomTypeStandardPrice(
        [FromRoute] long id,
        [FromRoute] long roomTypeId,
        [FromRoute] long siteId,
        [FromBody] RoomGroupPriceUpdateStandardPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        await Mediator!.Send(
            new PlanPriceUpdateStandardPriceCommand(id, roomTypeId, siteId) { Payload = request },
            cancellationToken
        );

        return NoContent();
    }

    [HttpPatch("children-price")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateRoomTypeChildrenPrice(
        [FromRoute] long id,
        [FromRoute] long roomTypeId,
        [FromRoute] long siteId,
        [FromBody] RoomGroupPriceUpdateChildrenPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        await Mediator!.Send(
            new PlanPriceUpdateChildrenPriceCommand(id, roomTypeId, siteId) { Payload = request },
            cancellationToken
        );

        return NoContent();
    }

    [HttpPatch("sale")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateRoomTypeSale(
        [FromRoute] long id,
        [FromRoute] long roomTypeId,
        [FromRoute] long siteId,
        [FromBody] RoomGroupPriceUpdateSaleRequest request,
        CancellationToken cancellationToken
    )
    {
        await Mediator!.Send(
            new PlanPriceUpdateSaleCommand(id, roomTypeId, siteId) { Payload = request },
            cancellationToken
        );

        return NoContent();
    }

    [HttpPatch("price-calendar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateRoomTypePriceCalendar(
        [FromRoute] long id,
        [FromRoute] long roomTypeId,
        [FromRoute] long siteId,
        [FromBody] RoomGroupPriceUpdatePriceCalendarRequest request,
        CancellationToken cancellationToken
    )
    {
        await Mediator!.Send(
            new PlanPriceUpdatePriceCalendarCommand(id, roomTypeId, siteId) { Payload = request },
            cancellationToken
        );

        return NoContent();
    }

    [HttpPatch("discount")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateRoomTypeDiscount(
        [FromRoute] long id,
        [FromRoute] long roomTypeId,
        [FromRoute] long siteId,
        [FromBody] RoomGroupPriceUpdateDiscountRequest request,
        CancellationToken cancellationToken
    )
    {
        await Mediator!.Send(
            new PlanPriceUpdateDiscountCommand(id, roomTypeId, siteId) { Payload = request },
            cancellationToken
        );

        return NoContent();
    }

    [HttpPatch("minimum-price")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanMinimumPriceAsync(
        [FromRoute] long id,
        [FromRoute] long roomTypeId,
        [FromRoute] long siteId,
        [FromBody] PlanRoomGroupSiteUpdateMinimumPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanRoomGroupSiteUpdateMinimumPriceCommand(id, roomTypeId, siteId) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Facility),
                    response.ToString()
                )
            );
    }

    [HttpGet("minimum-price")]
    [ProducesResponseType(typeof(PlanRoomSiteMinimumPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlanMinimumPriceAsync(
        [FromRoute] long id,
        [FromRoute] long roomTypeId,
        [FromRoute] long siteId,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanPriceGetMinimumPriceQuery(id, roomTypeId, siteId),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("minimum-price-summary")]
    [ProducesResponseType(typeof(PlanRoomSiteMinimumPriceSummaryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummaryMinimumPriceAsync(
        [FromRoute] long id,
        [FromRoute] long roomTypeId,
        [FromRoute] long siteId,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanPriceGetMinimumPriceSummaryQuery(id, roomTypeId, siteId),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("standard-price")]
    [ProducesResponseType(typeof(PlanRoomDetailStandardPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomTypeStandardPrice(
        [FromRoute] long id,
        [FromRoute] long roomTypeId,
        [FromRoute] long siteId,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanPriceGetStandardPriceQuery(id, roomTypeId, siteId),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("children-price")]
    [ProducesResponseType(typeof(PlanRoomDetailChildrenPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomTypeChildrenPrice(
        [FromRoute] long id,
        [FromRoute] long roomTypeId,
        [FromRoute] long siteId,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanPriceGetChildrenPriceQuery(id, roomTypeId, siteId),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("sale")]
    [ProducesResponseType(typeof(PlanRoomDetailSaleResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomTypeSale(
        [FromRoute] long id,
        [FromRoute] long roomTypeId,
        [FromRoute] long siteId,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanPriceGetSaleQuery(id, roomTypeId, siteId),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("price-calendar")]
    [ProducesResponseType(typeof(PlanRoomDetailPriceCalendarResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomTypePriceCalendar(
        [FromRoute] long id,
        [FromRoute] long roomTypeId,
        [FromRoute] long siteId,
        [FromQuery] long startDate,
        [FromQuery] long endDate,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanPriceGetPriceCalendarQuery(id, roomTypeId, siteId, startDate, endDate),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("discount")]
    [ProducesResponseType(typeof(PlanRoomDetailDiscountResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomTypeDiscount(
        [FromRoute] long id,
        [FromRoute] long roomTypeId,
        [FromRoute] long siteId,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanPriceGetDiscountQuery(id, roomTypeId, siteId),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("range-person")]
    [ProducesResponseType(typeof(PlanRoomSiteRangePersonResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRangePerson(
        [FromRoute] long id,
        [FromRoute] long roomTypeId,
        [FromRoute] long siteId,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanPriceGetRangePersonQuery(id, roomTypeId, siteId),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
