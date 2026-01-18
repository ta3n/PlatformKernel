using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Models;

public record PermissionFilterParameter(
    string? GroupName,
    string? ParentCode,
    ItemTypes? ItemTypes
);
