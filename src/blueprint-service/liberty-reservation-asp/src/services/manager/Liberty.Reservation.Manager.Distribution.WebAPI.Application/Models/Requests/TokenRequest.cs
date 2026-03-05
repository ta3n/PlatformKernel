namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;

public record TokenRequest(
    string ClientId,
    string ClientSecret,
    string GrantType,
    string Scope,
    string Username,
    string Password
);
