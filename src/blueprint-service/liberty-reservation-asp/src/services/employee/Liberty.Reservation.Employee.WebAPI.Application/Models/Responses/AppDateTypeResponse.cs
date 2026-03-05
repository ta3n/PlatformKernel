namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record AppDateTypeResponse(
    long Id,
    string Name,
    string ShortName,
    string Color,
    string? Description,
    bool IsEnabled
);
