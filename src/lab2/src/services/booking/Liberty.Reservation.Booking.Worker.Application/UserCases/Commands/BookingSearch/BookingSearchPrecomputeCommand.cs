using Liberty.Reservation.Booking.Worker.Application.Models.Requests;

namespace Liberty.Reservation.Booking.Worker.Application.UserCases.Commands.BookingSearch;

public record BookingSearchPrecomputeCommand : ActionCommandBase<
    IBookingSearchPrecomputeRequest,
    bool
>;
