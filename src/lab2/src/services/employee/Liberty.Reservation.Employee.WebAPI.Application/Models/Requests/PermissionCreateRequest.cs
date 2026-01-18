namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record PermissionCreateRequest(
    string? ParentCode,
    string? Key,
    string? GroupName,
    ItemTypes? ItemType,
    string? Name
);
