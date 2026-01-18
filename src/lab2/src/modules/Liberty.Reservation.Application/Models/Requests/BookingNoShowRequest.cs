namespace Liberty.Reservation.Application.Models.Requests;

public record BookingNoShowRequest(
    long? Id,
    string Reason
);
