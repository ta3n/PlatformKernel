using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupUpdateBasicConfigurationRequest(
    string Name,
    string? GroupName,
    string? Overview,
    string? Summary,
    string? Description,
    int? CapacityMin,
    int? CapacityMax,
    int? BaseNumber,
    float? Size,
    [property: JsonRequired] RoomGroupSizeUnitTypes RoomGroupSizeUnitType,
    IEnumerable<BedTypeOfRoomGroupUpdateBasicConfigurationRequest> BedTypes,
    [property: JsonRequired] bool IsEnabledSmoking,
    [property: JsonRequired] bool IsDescriptionVisible,
    [property: JsonRequired] bool IsOverviewVisible,
    [property: JsonRequired] bool IsRoomSizeVisible,
    [property: JsonRequired] bool IsBedTypeVisible,
    IEnumerable<FileOfRoomUpdateBasicSettingRequest> Files
)
{
    public long? Id { get; set; }
}

public record BedTypeOfRoomGroupUpdateBasicConfigurationRequest(
    long Id,
    int Number
);

public record FileOfRoomUpdateBasicSettingRequest(
    long Id,
    int Index
);
