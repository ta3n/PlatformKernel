namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record FacilityDetailFaxResponse(
    long Id,
    string? Fax,
    bool UseFax
);
