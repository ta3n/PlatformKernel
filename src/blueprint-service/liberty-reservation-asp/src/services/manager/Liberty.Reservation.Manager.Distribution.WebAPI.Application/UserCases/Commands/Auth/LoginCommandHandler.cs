using System.Net;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Options;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Commands.Auth;

public class LoginCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IHttpClientFactory httpClientFactory,
    IOptions<IdentitySetting> identitySettingOptions
) : ActionCommandHandlerBase<LoginCommand, IdentityResponse>(unitOfWork, mapper)
{
    private const string ConnectTokenEndpoint = "/connect/token";

    protected override async Task<IdentityResponse> HandleAsync(
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var jwtSetting = identitySettingOptions.Value.Jwt!;

        var tokenRequest = new TokenRequest(
            jwtSetting.ClientId ?? string.Empty,
            jwtSetting.ClientSecret ?? string.Empty,
            "password",
            jwtSetting.Scope ?? string.Empty,
            payload.Email,
            payload.Password
        );

        var requestUri = $"{jwtSetting.Authority}{ConnectTokenEndpoint}";
        var (statusCode, context) = await SendTokenRequestAsync(
            requestUri,
            tokenRequest,
            cancellationToken
        );

        return new IdentityResponse(
            statusCode,
            context
        );
    }

    private async Task<(HttpStatusCode statusCode, string context)> SendTokenRequestAsync(
        string requestUri,
        TokenRequest tokenRequest,
        CancellationToken cancellationToken
    )
    {
        using var client = httpClientFactory.CreateClient("IdentityClient");

        var requestContent = new FormUrlEncodedContent(
            [
                new("grant_type", tokenRequest.GrantType),
                new("client_id", tokenRequest.ClientId),
                new("client_secret", tokenRequest.ClientSecret),
                new("username", tokenRequest.Username),
                new("password", tokenRequest.Password),
                new("scope", tokenRequest.Scope)
            ]
        );

        var response = await client.PostAsync(
            requestUri,
            requestContent,
            cancellationToken
        );

        var context = await response.Content.ReadAsStringAsync(cancellationToken);

        return (response.StatusCode, context);
    }
}
