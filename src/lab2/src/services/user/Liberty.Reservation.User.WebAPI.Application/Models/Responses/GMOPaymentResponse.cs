namespace Liberty.Reservation.User.WebAPI.Application.Models.Responses;

public record GmoPaymentResponse(
    string? OrderId,
    string? OrderTime,
    decimal Amount,
    decimal? Tax,
    string? Url
);

public record OrderOfReservationResponse(
    string? OrderId,
    DateTime? OrderTime,
    decimal Amount,
    decimal? Tax,
    string? LanguageCode,
    long CheckInDate,
    string? TimeZone
);
