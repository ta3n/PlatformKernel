using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Commands.Kakusan;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Kakusan;
using static Liberty.Reservation.Application.Models.Responses.SetRoomsResponse;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Boundaries.Restful;

[Route("api/site-controller")]
[Produces("application/xml", "application/json")]
[Consumes("application/xml", "application/json")]
public class KakusanController(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpPost("GetRoomType")]
    public async Task<IActionResult> GetRoomType(
        [FromBody] RoomTypeRequest request,
        CancellationToken cancellationToken
    )
    {
        var (_, response) = await Mediator!.Send(
            new KakusanGetAllRoomTypeQuery(request),
            cancellationToken
        );

        if (response.ErrorCode is not null)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("GetRooms")]
    public async Task<IActionResult> GetRooms(
        [FromBody] GetRoomsRequest request,
        CancellationToken cancellationToken
    )
    {
        var (_, response) = await Mediator!.Send(
            new KakusanGetAllRoomQuery(request),
            cancellationToken
        );

        if (response.ErrorCode is not null)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("GetBooking")]
    public async Task<IActionResult> GetBooking(
        [FromBody] GetBookingRequest request,
        CancellationToken cancellationToken
    )
    {
        var (_, response) = await Mediator!.Send(
            new KakusanGetAllBookingQuery(request),
            cancellationToken
        );

        if (response.ErrorCode is not null)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("SetRooms")]
    public async Task<IActionResult> SetRooms(
        [FromBody] SetRoomsRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new KakusanSetRoomsCommand { Payload = request },
            cancellationToken
        );

        if (response.Result is EnumResultType.Success or EnumResultType.Processing)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpPost("Status")]
    public async Task<IActionResult> GetStatusRequest(
        [FromQuery] string code,
        CancellationToken cancellationToken
    )
    {
        var (_, response) = await Mediator!.Send(
            new KakusanGetStatusQuery(code),
            cancellationToken
        );

        return Ok(response);
    }
}
