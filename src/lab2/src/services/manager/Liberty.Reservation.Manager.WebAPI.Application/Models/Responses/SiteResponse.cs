namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record SiteResponse(
    long Id,
    string? Name,
    string? ShortName,
    string? Url,
    string? Description,
    bool IsEnabled
);
