namespace Liberty.Reservation.Application.Models;

public record FacilityStateModel(
    long Id,
    string? Code,
    bool IsOnLinePayment,
    bool CanOnLinePayment,
    bool IsOnSidePayment,
    bool UseDailyPerson,
    bool UseSpaTax
);
