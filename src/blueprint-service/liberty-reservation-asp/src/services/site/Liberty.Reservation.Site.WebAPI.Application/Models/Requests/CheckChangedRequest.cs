namespace Liberty.Reservation.Site.WebAPI.Application.Models.Requests;

public record CheckChangedRequest(
    string? LastUpdatedString,
    List<CheckUpdatedOptionItemRequest> OptionItems
);

public record CheckUpdatedOptionItemRequest(
    long Id,
    string? LastUpdatedAt
);
