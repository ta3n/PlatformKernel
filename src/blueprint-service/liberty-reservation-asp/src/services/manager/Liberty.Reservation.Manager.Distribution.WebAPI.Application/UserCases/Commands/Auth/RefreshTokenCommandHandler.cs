using System.Net;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Options;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Commands.Auth;

public class RefreshTokenCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IHttpClientFactory httpClientFactory,
    IOptions<IdentitySetting> identitySettingOptions
) : ActionCommandHandlerBase<RefreshTokenCommand, IdentityResponse>(unitOfWork, mapper)
{
    private const string ConnectTokenEndpoint = "/connect/token";

    protected override async Task<IdentityResponse> HandleAsync(
        RefreshTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var jwtSetting = identitySettingOptions.Value.Jwt!;

        var tokenRequest = new RefreshTokenRequest(
            jwtSetting.ClientId ?? string.Empty,
            jwtSetting.ClientSecret ?? string.Empty,
            "refresh_token",
            payload.RefreshToken
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
        RefreshTokenRequest refreshTokenRequest,
        CancellationToken cancellationToken
    )
    {
        using var client = httpClientFactory.CreateClient("IdentityClient");

        var requestContent = new FormUrlEncodedContent(
            [
                new("grant_type", refreshTokenRequest.GrantType),
                new("client_id", refreshTokenRequest.ClientId),
                new("client_secret", refreshTokenRequest.ClientSecret),
                new("refresh_token", refreshTokenRequest.RefreshToken)
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
