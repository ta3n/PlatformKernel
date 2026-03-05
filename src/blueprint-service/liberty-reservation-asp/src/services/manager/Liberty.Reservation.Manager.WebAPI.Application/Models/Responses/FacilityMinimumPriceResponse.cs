namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record FacilityMinimumPriceResponse(
    string? Code,
    bool IsEnabledMinimumPrice,
    int? MinimumPrice
);
