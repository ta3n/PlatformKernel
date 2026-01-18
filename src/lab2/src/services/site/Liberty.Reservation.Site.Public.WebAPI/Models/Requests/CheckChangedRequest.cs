namespace Liberty.Reservation.Site.Public.WebAPI.Models.Requests;

public record CheckChangedRequest(
    string? LastUpdatedString,
    List<CheckUpdatedOptionItemRequest> OptionItems
);

public record CheckUpdatedOptionItemRequest(
    long Id,
    string? LastUpdatedAt
);
