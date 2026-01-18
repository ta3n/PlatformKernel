namespace Liberty.Reservation.Application.Models.Requests;

public record BookingSecureUrlRequest(
    string BookingId,
    int ValidMinutes
);
