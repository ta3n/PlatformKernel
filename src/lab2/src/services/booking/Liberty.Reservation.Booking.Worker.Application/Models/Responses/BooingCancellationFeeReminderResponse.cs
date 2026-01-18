namespace Liberty.Reservation.Booking.Worker.Application.Models.Responses;

public record BooingCancellationFeeReminderResponse(
    long[] Processed,
    long[] Errors
);
