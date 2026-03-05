namespace Liberty.Reservation.Application.Models.Responses;

public record BookingAdjustResponse(
    long AdjustBookingId,
    bool IsModifyInPrice,
    decimal CancellationPrice
);
