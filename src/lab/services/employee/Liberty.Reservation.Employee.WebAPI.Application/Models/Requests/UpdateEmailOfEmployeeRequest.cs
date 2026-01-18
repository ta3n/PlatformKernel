namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record UpdateEmailOfEmployeeRequest(
    string Id,
    string? EMail
);
