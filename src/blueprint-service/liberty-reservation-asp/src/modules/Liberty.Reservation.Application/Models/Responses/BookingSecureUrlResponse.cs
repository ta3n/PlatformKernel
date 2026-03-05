using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Application.Models.Responses;

public record BookingSecureUrlResponse(
    bool IsSuccess,
    BookingSecureUrlRequest? Request
);
