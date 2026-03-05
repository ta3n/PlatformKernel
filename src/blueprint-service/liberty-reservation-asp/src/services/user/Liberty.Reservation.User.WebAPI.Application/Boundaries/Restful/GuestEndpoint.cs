using Liberty.Pagination;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.User.WebAPI.Application.Models.Requests;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;
using Microsoft.AspNetCore.Authorization;

namespace Liberty.Reservation.User.WebAPI.Application.Boundaries.Restful;

[Route("api/guest/booking/{code}")]
[AllowAnonymous]
public class GuestEndpoint(
    IMapper mapper,
    IMediator mediator,
    IBookingSecureUrlService bookingSecureUrlService
) : BaseEndpoint(mapper, mediator)
{
    [HttpPatch("confirm")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ConfirmOfReservation(
        [FromRoute] string code,
        CancellationToken cancellationToken
    )
    {
        var (_, booking) = bookingSecureUrlService.DecryptAndValidate(code);
        var request = new BookingConfirmRequest
        {
            Id = Convert.ToInt64(booking?.BookingId),
            GuestCode = code
        };

        var response = await Mediator!.Send(
            new BookingConfirmCommand { Payload = request },
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

    [HttpPatch("change-execution")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangeExecutionOfReservation(
        [FromRoute] string code,
        [FromBody] BookingAdjustRequest request,
        CancellationToken cancellationToken
    )
    {
        var (_, booking) = bookingSecureUrlService.DecryptAndValidate(code);
        request.Id = Convert.ToInt64(booking?.BookingId);
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

    [HttpPatch("cancellation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CancellationOfReservation(
        [FromRoute] string code,
        [FromBody] BookingCancellationRequest request,
        CancellationToken cancellationToken
    )
    {
        var (_, booking) = bookingSecureUrlService.DecryptAndValidate(code);
        request.Id = Convert.ToInt64(booking?.BookingId);

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

    [HttpGet("cancellation-fee")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCancellationFee(
        [FromRoute] string code,
        CancellationToken cancellationToken
    )
    {
        var (_, booking) = bookingSecureUrlService.DecryptAndValidate(code);
        var id = Convert.ToInt64(booking?.BookingId);

        var (headers, response) = await Mediator!.Send(
            new ReservationGetCancellationFeeQuery(
                id
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("")]
    [ProducesResponseType(typeof(ReservationDetailsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReservationDetails(
        [FromRoute] string code,
        CancellationToken cancellationToken
    )
    {
        var (_, booking) = bookingSecureUrlService.DecryptAndValidate(code);
        var id = Convert.ToInt64(booking?.BookingId);

        var (headers, response) = await Mediator!.Send(
            new ReservationGetDetailsQuery(id),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("person-age-types")]
    [ProducesResponseType(typeof(List<PersonAgeTypeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPersonAgeTypesOfReservation(
        [FromRoute] string code,
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var (_, booking) = bookingSecureUrlService.DecryptAndValidate(code);
        var id = Convert.ToInt64(booking?.BookingId);

        var (headers, response) = await Mediator!.Send(
            new ReservationGetAllPersonAgeTypesQuery(
                id,
                pageable
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("option-items")]
    [ProducesResponseType(typeof(List<OptionItemOfPlanResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllOptionItemsOfPlan(
        IPageable pageable,
        [FromRoute] string code,
        [FromQuery] int appDateId,
        [FromQuery] int roomGroupIndex,
        CancellationToken cancellationToken
    )
    {
        var (_, booking) = bookingSecureUrlService.DecryptAndValidate(code);

        var id = Convert.ToInt64(booking?.BookingId);

        var (headers, response) = await Mediator!.Send(
            new ReservationGetAllOptionItemsQuery(id, appDateId, roomGroupIndex, pageable),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPost("check-night-number")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckNightNumber(
        [FromRoute] string code,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        var (_, booking) = bookingSecureUrlService.DecryptAndValidate(code);
        var id = Convert.ToInt64(booking?.BookingId);

        var response = await Mediator!.Send(
            new BookingCheckNightNumberCommand(
                id
            ) { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPost("check-room-number")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckRoomNumber(
        [FromRoute] string code,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        var (_, booking) = bookingSecureUrlService.DecryptAndValidate(code);
        var id = Convert.ToInt64(booking?.BookingId);

        var response = await Mediator!.Send(
            new BookingCheckRoomNumberCommand(
                id
            ) { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPost("change-persons")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangePersonsAsync(
        [FromRoute] string code,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        var (_, booking) = bookingSecureUrlService.DecryptAndValidate(code);
        var id = Convert.ToInt64(booking?.BookingId);

        var response = await Mediator!.Send(
            new ChangePersonsBookingCommand(
                id
            ) { Payload = request },
            cancellationToken
        );
        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPost("adjust-options")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AdjustOptionsAsync(
        [FromRoute] string code,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        var (_, booking) = bookingSecureUrlService.DecryptAndValidate(code);
        var id = Convert.ToInt64(booking?.BookingId);

        var response = await Mediator!.Send(
            new AdjustOptionsCommand(
                id
            ) { Payload = request },
            cancellationToken
        );
        return ActionResultUtil.WrapOrNotFound(response);
    }
}
