using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;

namespace Liberty.Reservation.Site.WebAPI.Application.Boundaries.Restful;

public partial class BookingEndpoint
{
    private async Task CheckBookingByComboAsync(
        long[] planIds,
        long[] roomGroupIds,
        CancellationToken cancellationToken
    )
    {
        var isAvailable = await planRoomGroupService.IsAvailablePlanWithRoomAsync(
            planIds,
            roomGroupIds,
            securityContextAccessor.GetFacilityIdSelected(),
            PlanTypes.Combo,
            cancellationToken
        );
        if (isAvailable is false)
        {
            throw new PlanNotfoundException();
        }
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(List<BookingSearchByPlanResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchBooking(
        [FromQuery] IPageable pageable,
        [FromBody] BookingSearchPlanRequest request,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new BookingSearchByPlanQuery(request, pageable),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("plans/{planId:min(1)}/rooms/{roomGroupId:min(1)}")]
    [ProducesResponseType(typeof(BookingDetailsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBookingDetails(
        [FromRoute] long planId,
        [FromRoute] long roomGroupId,
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

    [HttpPost("plans/{planId:min(1)}/rooms/{roomGroupId:min(1)}/price-calendar")]
    [ProducesResponseType(typeof(PriceCalendarOfRoomResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBookingPriceCalendar(
        [FromRoute] long planId,
        [FromRoute] long roomGroupId,
        [FromBody] BookingSearchPlanRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByComboAsync(
            [planId],
            [roomGroupId],
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

    [HttpPost("plans/{planId:min(1)}/rooms/{roomGroupId:min(1)}/check-night-number")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckNightNumber(
        [FromRoute] long planId,
        [FromRoute] long roomGroupId,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByComboAsync(
            [planId],
            [roomGroupId],
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

    [HttpPost("plans/{planId:min(1)}/rooms/{roomGroupId:min(1)}/check-room-number")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckRoomNumber(
        [FromRoute] long planId,
        [FromRoute] long roomGroupId,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByComboAsync(
            [planId],
            [roomGroupId],
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

    [HttpGet("plans/{planId:min(1)}/option-items")]
    [ProducesResponseType(typeof(List<OptionItemOfBookingResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllOptionItemsOfPlan(
        [FromRoute] long planId,
        [FromQuery] BookingOptionRequest request,
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
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

    [HttpPost("plans/{planId:min(1)}/rooms/{roomGroupId:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateBookingAsync(
        [FromRoute] long planId,
        [FromRoute] long roomGroupId,
        [FromBody] SiteBookingCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByComboAsync(
            [planId],
            [roomGroupId],
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

    [HttpPost("plans/{planId:min(1)}/rooms/{roomGroupId:min(1)}/change-persons")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangePersonsAsync(
        [FromRoute] long planId,
        [FromRoute] long roomGroupId,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByComboAsync(
            [planId],
            [roomGroupId],
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

    [HttpPost("plans/{planId:min(1)}/rooms/{roomGroupId:min(1)}/adjust-options")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AdjustOptionsAsync(
        [FromRoute] long planId,
        [FromRoute] long roomGroupId,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByComboAsync(
            [planId],
            [roomGroupId],
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

    [HttpPost("plans/{planId:min(1)}/rooms/{roomGroupId:min(1)}/check-changed")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckChangedAsync(
        [FromRoute] long planId,
        [FromRoute] long roomGroupId,
        [FromBody] CheckChangedRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByComboAsync(
            [planId],
            [roomGroupId],
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
