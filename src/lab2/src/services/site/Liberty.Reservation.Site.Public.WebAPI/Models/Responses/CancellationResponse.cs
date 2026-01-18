namespace Liberty.Reservation.Site.Public.WebAPI.Models.Responses;

public record CancellationResponse(
    long? Id,
    string? Name,
    string? Description,
    string? TableSource
);
