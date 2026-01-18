namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record UpdateEmployeeRequest(
    string Id,
    string? Name,
    string? Kana
);
