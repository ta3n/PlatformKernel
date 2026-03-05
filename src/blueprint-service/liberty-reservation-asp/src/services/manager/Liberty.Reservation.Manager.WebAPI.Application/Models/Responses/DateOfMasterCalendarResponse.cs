namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record DateOfMasterCalendarResponse(
    long AppDate,
    string? TypeName,
    string? TypeShortName,
    string? TypeColor,
    string? DataName
);
