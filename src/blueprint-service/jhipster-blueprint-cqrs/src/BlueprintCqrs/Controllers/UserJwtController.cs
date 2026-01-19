using MediatR;
using BlueprintCqrs.Security.Jwt;
using BlueprintCqrs.Dto.Authentication;
using BlueprintCqrs.Web.Extensions;
using BlueprintCqrs.Crosscutting.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using BlueprintCqrs.Application.Commands.UserJwt;

namespace BlueprintCqrs.Controllers;

[Route("api")]
[ApiController]
public class UserJwtController(
    IMediator mediator,
    ITokenProvider tokenProvider
) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ITokenProvider _tokenProvider = tokenProvider;

    [HttpPost("authenticate")]
    public async Task<ActionResult<JwtToken>> Authorize(
        [FromBody] LoginDto loginDto
    )
    {
        var user = await _mediator.Send(new UserJwtAuthorizeCommand { LoginDto = loginDto });
        var rememberMe = loginDto.RememberMe;
        var jwt = _tokenProvider.CreateToken(user, rememberMe);
        var httpHeaders = new HeaderDictionary { [JwtConstants.AuthorizationHeader] = $"{JwtConstants.BearerPrefix} {jwt}" };
        return Ok(new JwtToken(jwt)).WithHeaders(httpHeaders);
    }
}

public class JwtToken(
    string idToken
)
{
    [JsonProperty("id_token")]
    private string IdToken { get; } = idToken;
}
