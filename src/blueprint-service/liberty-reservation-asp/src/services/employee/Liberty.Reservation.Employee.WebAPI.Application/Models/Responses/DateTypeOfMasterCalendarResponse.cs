namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record DateTypeOfMasterCalendarResponse(
    long AppDate,
    string? Code,
    string? Name,
    string? ShortName,
    string? Color
);
