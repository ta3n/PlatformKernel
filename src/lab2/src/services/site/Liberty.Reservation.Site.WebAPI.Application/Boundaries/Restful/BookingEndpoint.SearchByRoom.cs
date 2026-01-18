using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Site.Application.Models.Responses;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;

namespace Liberty.Reservation.Site.WebAPI.Application.Boundaries.Restful;

public partial class BookingEndpoint
{
    private async Task CheckBookingByRoomOnlyAsync(
        long[] roomGroupIds,
        long[] planIds,
        CancellationToken cancellationToken
    )
    {
        var isAvailable = await planRoomGroupService.IsAvailablePlanWithRoomAsync(
            planIds,
            roomGroupIds,
            securityContextAccessor.GetFacilityIdSelected(),
            PlanTypes.RoomOnly,
            cancellationToken
        );
        if (isAvailable is false)
        {
            throw new PlanNotfoundException();
        }
    }

    [HttpPost("search-by-room")]
    [ProducesResponseType(typeof(List<BookingSearchByRoomResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchBookingByRoom(
        [FromQuery] IPageable pageable,
        [FromBody] BookingSearchPlanRequest request,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new BookingSearchByRoomQuery(request, pageable),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("rooms/{roomGroupId:min(1)}/plans/{planId:min(1)}")]
    [ProducesResponseType(typeof(BookingDetailsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBookingRoomDetails(
        [FromRoute] long roomGroupId,
        [FromRoute] long planId,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new BookingGetDetailsQuery(
                planId,
                roomGroupId
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPost("rooms/{roomGroupId:min(1)}/plans/{planId:min(1)}/price-calendar")]
    [ProducesResponseType(typeof(PriceCalendarOfRoomResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBookingRoomPriceCalendar(
        [FromRoute] long roomGroupId,
        [FromRoute] long planId,
        [FromBody] BookingSearchPlanRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByRoomOnlyAsync(
            [roomGroupId],
            [planId],
            cancellationToken
        );

        var (headers, response) = await Mediator!.Send(
            new BookingGetCalendarPricesQuery(
                planId,
                roomGroupId,
                request
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPost("rooms/{roomGroupId:min(1)}/plans/{planId:min(1)}/check-night-number")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckNightNumberRoom(
        [FromRoute] long roomGroupId,
        [FromRoute] long planId,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByRoomOnlyAsync(
            [roomGroupId],
            [planId],
            cancellationToken
        );

        var response = await Mediator!.Send(
            new BookingCheckNightNumberCommand(
                planId,
                roomGroupId
            ) { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPost("rooms/{roomGroupId:min(1)}/plans/{planId:min(1)}/check-room-number")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckRoomNumberOfRoomGroup(
        [FromRoute] long roomGroupId,
        [FromRoute] long planId,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByRoomOnlyAsync(
            [roomGroupId],
            [planId],
            cancellationToken
        );

        var response = await Mediator!.Send(
            new BookingCheckRoomNumberCommand(
                planId,
                roomGroupId
            ) { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpGet("rooms/{roomGroupId:min(1)}/plans/{planId:min(1)}/option-items")]
    [ProducesResponseType(typeof(List<OptionItemOfBookingResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllOptionItemsOfRoom(
        [FromRoute] long roomGroupId,
        [FromRoute] long planId,
        [FromQuery] BookingOptionRequest request,
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByRoomOnlyAsync(
            [roomGroupId],
            [planId],
            cancellationToken
        );

        var (headers, response) = await Mediator!.Send(
            new BookingGetAllOptionItemsQuery(
                planId,
                request,
                pageable
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPost("rooms/{roomGroupId:min(1)}/plans/{planId:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateBookingRoomAsync(
        [FromRoute] long roomGroupId,
        [FromRoute] long planId,
        [FromBody] SiteBookingCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByRoomOnlyAsync(
            [roomGroupId],
            [planId],
            cancellationToken
        );

        var response = await Mediator!.Send(
            new BookingCreateCommand(
                planId,
                roomGroupId
            ) { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response)
            .WithHeaders(
                HeaderUtil.CreateEntityCreationAlert(
                    nameof(BookingCreateRequest),
                    response
                )
            );
    }

    [HttpPost("rooms/{roomGroupId:min(1)}/plans/{planId:min(1)}/change-persons")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangePersonsOfRoomAsync(
        [FromRoute] long roomGroupId,
        [FromRoute] long planId,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByRoomOnlyAsync(
            [roomGroupId],
            [planId],
            cancellationToken
        );

        var response = await Mediator!.Send(
            new ChangePersonsBookingCommand(
                planId,
                roomGroupId
            ) { Payload = request },
            cancellationToken
        );
        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPost("rooms/{roomGroupId:min(1)}/plans/{planId:min(1)}/adjust-options")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AdjustOptionsOfRoomAsync(
        [FromRoute] long roomGroupId,
        [FromRoute] long planId,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByRoomOnlyAsync(
            [roomGroupId],
            [planId],
            cancellationToken
        );

        var response = await Mediator!.Send(
            new AdjustOptionsCommand(
                planId,
                roomGroupId
            ) { Payload = request },
            cancellationToken
        );
        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPost("rooms/{roomGroupId:min(1)}/plans/{planId:min(1)}/check-changed")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckChangedOfRoomAsync(
        [FromRoute] long roomGroupId,
        [FromRoute] long planId,
        [FromBody] CheckChangedRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByRoomOnlyAsync(
            [roomGroupId],
            [planId],
            cancellationToken
        );

        var response = await Mediator!.Send(
            new CheckChangedCommand(
                planId,
                roomGroupId
            ) { Payload = request },
            cancellationToken
        );
        return ActionResultUtil.WrapOrNotFound(response);
    }
}
