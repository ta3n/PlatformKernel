using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Commands.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Boundaries.Restful;

[AllowAnonymous]
[Route("api/auth/token")]
public class AuthEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpPost("")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var (statusCode, context) = await Mediator!.Send(
            new LoginCommand { Payload = request },
            cancellationToken
        );

        return StatusCode((int)statusCode, context);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshRequest request,
        CancellationToken cancellationToken
    )
    {
        var (statusCode, context) = await Mediator!.Send(
            new RefreshTokenCommand { Payload = request },
            cancellationToken
        );

        return StatusCode((int)statusCode, context);
    }
}
