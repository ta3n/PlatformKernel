namespace Liberty.Reservation.Application.Models.Responses;

public record BookingCancellationFeeResponse(
    long ReservationId,
    decimal BookingTotalPrice,
    decimal CancellationPrice,
    float RateFee,
    bool HasPolicy
);
