namespace Liberty.Reservation.Manager.Application.Models;

public record FacilityMinimumPriceData(
    bool IsEnabledMinimumPrice,
    int? MinimumPrice
);
