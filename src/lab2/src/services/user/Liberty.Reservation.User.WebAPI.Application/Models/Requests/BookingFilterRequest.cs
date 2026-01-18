using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.User.WebAPI.Application.Models.Requests;

public record BookingFilterRequest(
    UserBookingFilter? UserBookingFilter
);
