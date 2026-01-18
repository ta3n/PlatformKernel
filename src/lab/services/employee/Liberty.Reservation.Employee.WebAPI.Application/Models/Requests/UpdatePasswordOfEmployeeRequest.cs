namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record UpdatePasswordOfEmployeeRequest(
    string Id,
    string Password
);
