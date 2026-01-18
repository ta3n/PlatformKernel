namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PriceTypeResponse(
    long Id,
    string? Name,
    string? ShortName,
    string? Color,
    long DisplayOrder,
    bool IsEnabled
);
