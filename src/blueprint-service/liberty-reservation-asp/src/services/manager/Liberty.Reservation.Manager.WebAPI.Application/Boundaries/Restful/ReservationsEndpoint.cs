using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BookingReservation;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/reservations")]
public class ReservationsEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpPatch("{id:long:min(1)}/change-execution")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangeExecutionReservation(
        [FromRoute] long id,
        [FromBody] BookingAdjustRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await Mediator!.Send(
            new BookingChangeExecutionCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Reservation),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/cancellation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CancellationReservation(
        [FromRoute] long id,
        [FromBody] BookingCancellationByManagerRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await Mediator!.Send(
            new BookingCancellationCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Reservation),
                    response.ToString()
                )
            );
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(List<BookingReservationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchReservations(
        [FromQuery] IPageable pageable,
        [FromBody] ReservationGetAllBylFilterRequest request,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new ReservationGetAllByFilterQuery(
                request,
                pageable
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("filter-options")]
    [ProducesResponseType(typeof(ReservationFilterOptionsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilterOptions()
    {
        var (headers, response) = await Mediator!.Send(
            new ReservationGetFilterOptionsQuery(
            )
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}")]
    [ProducesResponseType(typeof(ReservationDetailResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReservationDetails(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new ReservationGetDetailsQuery(id),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/option-items")]
    [ProducesResponseType(typeof(List<OptionItemOfPlanResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllOptionItemsOfPlan(
        IPageable pageable,
        [FromRoute] long id,
        [FromQuery] int appDateId,
        [FromQuery] int roomGroupIndex,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new ReservationGetAllOptionItemsQuery(id, appDateId, roomGroupIndex, pageable),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPost("{id:long:min(1)}/check-night-number")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckNightNumber(
        [FromRoute] long id,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new BookingCheckNightNumberCommand(
                id
            ) { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpGet("{id:long:min(1)}/cancellation-fee")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCancellationFee(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new ReservationGetCancellationFeeQuery(
                id
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPost("{id:long:min(1)}/check-room-number")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckRoomNumber(
        [FromRoute] long id,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new BookingCheckRoomNumberCommand(
                id
            ) { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPost("{id:long:min(1)}/change-persons")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangePersonsAsync(
        [FromRoute] long id,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new ChangePersonsBookingCommand(
                id
            ) { Payload = request },
            cancellationToken
        );
        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPost("{id:long:min(1)}/adjust-options")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AdjustOptionsAsync(
        [FromRoute] long id,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new AdjustOptionsCommand(
                id
            ) { Payload = request },
            cancellationToken
        );
        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpGet("months")]
    [ProducesResponseType(typeof(IEnumerable<long>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMonthsOfReservations(
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new ReservationGetMonthsQuery(),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/person-age-types")]
    [ProducesResponseType(typeof(List<PersonAgeTypeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPersonAgeTypesOfReservation(
        IPageable pageable,
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new ReservationGetAllPersonAgeTypesQuery(
                id,
                pageable
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPatch("{id:long:min(1)}/no-show")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> NoShowReservation(
        [FromRoute] long id,
        [FromBody] BookingNoShowRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new BookingNoShowCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Reservation),
                    response.ToString()
                )
            );
    }

    [HttpGet("{id:long:min(1)}/histories")]
    [ProducesResponseType(typeof(List<BookingAuditLogResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllHistoriesOfReservation(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new BookingGetAllAuditLogQuery(
                id,
                PageableConstants.UnPaged
            ),
            cancellationToken
        );

        return !response.Any()
            ? NoContent().WithHeaders(headers)
            : ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
