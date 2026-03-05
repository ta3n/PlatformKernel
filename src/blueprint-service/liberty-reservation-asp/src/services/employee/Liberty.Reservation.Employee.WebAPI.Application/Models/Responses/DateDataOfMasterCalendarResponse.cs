namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record DateDataOfMasterCalendarResponse(
    long AppDate,
    string? Name
);
