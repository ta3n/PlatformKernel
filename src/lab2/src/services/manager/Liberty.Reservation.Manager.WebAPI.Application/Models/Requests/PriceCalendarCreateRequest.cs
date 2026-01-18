namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PriceCalendarCreateRequest(
    List<FacilityCalendarCreateRequest> Calendars
);

public record FacilityCalendarCreateRequest(
    long PriceTypeId,
    int DateCalendar,
    bool IsDeleted
);
