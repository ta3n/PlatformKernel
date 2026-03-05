namespace Liberty.Reservation.Application.Models;

public record FacilityStateModel(
    long Id,
    bool IsOnLinePayment,
    bool CanOnLinePayment,
    bool IsOnSidePayment,
    bool UseDailyPerson,
    bool UseSpaTax
);
