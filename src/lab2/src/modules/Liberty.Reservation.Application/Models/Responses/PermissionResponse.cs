using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Models.Responses;

public record PermissionResponse(
    string? Key,
    string? GroupName,
    string? ParentCode,
    ItemTypes ItemType,
    string? Name,
    long DisplayOrder,
    bool IsEnabled,
    List<PermissionResponse>? Items
)
{
    public string? Path { get; set; }
}

