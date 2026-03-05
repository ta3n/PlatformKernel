namespace Liberty.Reservation.Booking.Worker.Application.Models.Responses;

public record BookingReminderOfUpcomingCheckInDateResponse(
    long[] Processed,
    long[] Errors
);
