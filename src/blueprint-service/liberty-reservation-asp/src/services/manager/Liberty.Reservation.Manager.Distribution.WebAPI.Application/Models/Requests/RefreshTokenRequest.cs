namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;

public record RefreshTokenRequest(
    string ClientId,
    string ClientSecret,
    string GrantType,
    string RefreshToken
);
