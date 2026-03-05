using Liberty.Pagination;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.User.WebAPI.Application.Models.Requests;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;
using Microsoft.AspNetCore.Authorization;

namespace Liberty.Reservation.User.WebAPI.Application.Boundaries.Restful;

[Route("api/reservations")]
public class ReservationsEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpPatch("{id:long:min(1)}/change-execution")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangeExecutionOfReservation(
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
    public async Task<IActionResult> CancellationOfReservation(
        [FromRoute] long id,
        [FromBody] BookingCancellationRequest request,
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

    [HttpGet]
    [ProducesResponseType(typeof(List<ReservationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllReservationsByFilter(
        IPageable pageable,
        [FromQuery] BookingFilterRequest request,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new ReservationGetAllByFilterQuery(pageable, request),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}")]
    [ProducesResponseType(typeof(ReservationDetailsResponse), StatusCodes.Status200OK)]
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

    [HttpGet("{id:long:min(1)}/payment-info")]
    [ProducesResponseType(typeof(GmoPaymentResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReservationGmoPaymentById(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new ReservationGmoPaymentQuery(id),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{code}/payment-info")]
    [ProducesResponseType(typeof(GmoPaymentResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReservationGmoPaymentByCode(
        [FromRoute] string code,
        [FromQuery] bool isSiteLocation,
        CancellationToken cancellationToken
    )
    {
        var id = await Mediator!.Send(
            new BookingChangeLocationCommand { Payload = new BookingChangeLocationRequest(code, isSiteLocation) },
            cancellationToken
        );

        var (headers, response) = await Mediator!.Send(
            new ReservationGmoPaymentQuery(id),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [AllowAnonymous]
    [HttpPost("/gmo-payment-result")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CreateGmoPaymentResult(
        [FromForm] GmoPaymentLinkPlusRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new BookingGmoPaymentCommand { Payload = request },
            cancellationToken
        );

        return Redirect(response);
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
}
