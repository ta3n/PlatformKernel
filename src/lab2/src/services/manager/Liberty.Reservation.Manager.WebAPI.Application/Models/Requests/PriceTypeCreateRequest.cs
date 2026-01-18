namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PriceTypeCreateRequest(
    string? ShortName,
    string? Name,
    string? Color
);
