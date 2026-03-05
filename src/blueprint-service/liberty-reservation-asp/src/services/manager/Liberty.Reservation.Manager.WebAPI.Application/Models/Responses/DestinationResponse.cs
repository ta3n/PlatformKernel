namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record DestinationResponse(
    long Id,
    string Code,
    string Name,
    string ShortName,
    string? Url,
    long DisplayOrder,
    bool IsEnabled
);
