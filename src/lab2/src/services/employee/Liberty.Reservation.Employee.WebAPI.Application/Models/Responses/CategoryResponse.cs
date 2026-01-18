namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record CategoryResponse(
    long Id,
    string Code,
    string Name,
    string? Description,
    bool IsEnabled
);
