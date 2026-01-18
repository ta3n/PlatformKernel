namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record RoomGroupDetailBasicConfigurationResponse(
    long Id,
    string Name,
    string GroupName,
    string? Description,
    int CapacityMin,
    int CapacityMax,
    int BaseNumber,
    decimal? Size,
    RoomGroupSizeUnitTypes RoomGroupSizeUnitType,
    BedTypeOfRoomGroupUpdateBasicConfigurationResponse[] BedTypes,
    bool IsEnabledSmoking,
    bool IsDescriptionVisible,
    bool IsOverviewVisible,
    bool IsRoomSizeVisible,
    bool IsBedTypeVisible,
    IEnumerable<FileOfRoomBasicConfigurationResponse> Files
)
{
    public string? Overview { get; set; }
    public string? Summary { get; set; }
}

public record BedTypeOfRoomGroupUpdateBasicConfigurationResponse(
    long Id,
    string Code,
    string Name,
    int Number
);

public record FileOfRoomBasicConfigurationResponse(
    long Id,
    string? Code,
    int Index,
    bool IsEnabled,
    string? Description
);
