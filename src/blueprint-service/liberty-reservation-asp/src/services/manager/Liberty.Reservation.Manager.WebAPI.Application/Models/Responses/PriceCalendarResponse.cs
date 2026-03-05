namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PriceCalendarResponse(
    long PriceTypeId,
    int DateCalendar
);
