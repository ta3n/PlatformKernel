namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record AppDateTypeCreateRequest(
    string Name,
    string ShortName,
    string Color,
    string? Description
);
