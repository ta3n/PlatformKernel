namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record DateOfMasterCalendarResponse(
    long AppDate,
    string? TypeName,
    string? TypeShortName,
    string? TypeColor,
    string? DataName
);
