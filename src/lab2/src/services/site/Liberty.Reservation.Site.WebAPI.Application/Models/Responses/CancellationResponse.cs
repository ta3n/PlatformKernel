namespace Liberty.Reservation.Site.WebAPI.Application.Models.Responses;

public record CancellationResponse(
    long? Id,
    string? Code,
    string? Name,
    string? Description,
    string? TableSource
);
