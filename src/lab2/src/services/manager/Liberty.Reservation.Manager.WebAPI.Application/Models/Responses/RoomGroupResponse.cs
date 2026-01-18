namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record RoomGroupResponse(
    long Id,
    string Code,
    string Name,
    string? GroupName,
    int CapacityMin,
    int CapacityMax,
    int BaseNumber,
    decimal Size,
    RoomGroupSizeUnitTypes RoomGroupSizeUnitType,
    long DisplayOrder,
    bool IsEnabledSmoking,
    bool IsEnabled,
    IEnumerable<ImageOfRoomGroupDetailMediaSettingResponse> Images,
    string? Tag
);
