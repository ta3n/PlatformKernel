namespace Liberty.Reservation.Employee.WebAPI.Application.Models;

public record PermissionFlatDto(
    string? Key,
    string? ParentCode,
    bool IsEnabled
);

