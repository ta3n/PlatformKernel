namespace Liberty.Reservation.User.WebAPI.Application.Models.Requests;

public record BookingChangeLocationRequest(
    string Code,
    bool IsSiteLocation
);
