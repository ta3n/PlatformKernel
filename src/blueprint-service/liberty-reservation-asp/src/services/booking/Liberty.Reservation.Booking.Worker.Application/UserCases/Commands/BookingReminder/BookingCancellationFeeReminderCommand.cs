using Liberty.Reservation.Booking.Worker.Application.Models.Requests;
using Liberty.Reservation.Booking.Worker.Application.Models.Responses;

namespace Liberty.Reservation.Booking.Worker.Application.UserCases.Commands.BookingReminder;

public record BookingCancellationFeeReminderCommand : ActionCommandBase<
    IBooingCancellationFeeReminderRequest,
    BooingCancellationFeeReminderResponse
>;
