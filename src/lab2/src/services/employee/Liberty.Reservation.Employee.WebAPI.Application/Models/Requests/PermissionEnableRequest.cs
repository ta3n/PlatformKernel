using Newtonsoft.Json;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record PermissionEnableRequest(
    [property: JsonRequired] string? Key,
    string? GroupName,
     string? ParentCode,
    [property: JsonRequired] ItemTypes ItemType,
    string? Name,
    long? DisplayOrder,
    [property: JsonRequired] bool IsEnabled,
    string? Path,
    List<PermissionEnableRequest>? Items
);
