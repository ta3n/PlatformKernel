namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PriceCalendarResponse(
    long PriceTypeId,
    int DateCalendar,
    string? Name,
    string? ShortName,
    string? Color,
    long DisplayOrder,
    bool IsEnabled
);
