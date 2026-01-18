namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record CategoryCreateRequest(
    string? Name,
    string? Description
);
