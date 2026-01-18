namespace Liberty.Reservation.Application.Models.Responses;

public record BookingAbortResponse(
    long BookingId,
    decimal CancellationPrice,
    DateTime CancellationTime,
    float CancellationRate
);
