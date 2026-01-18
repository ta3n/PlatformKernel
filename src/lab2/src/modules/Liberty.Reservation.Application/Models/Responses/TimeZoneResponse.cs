namespace Liberty.Reservation.Application.Models.Responses;

public record TimeZoneResponse(
    string Name,
    TimeSpan UtcOffset
);
