namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record UpdatePasswordOfSelfRequest(
    string Id,
    string Password
);
