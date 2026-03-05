namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record RoomGroupDetailDisplaySettingResponse
{
    public long Id { get; init; }
    public IEnumerable<CategoryOfRoomGroupDetailDisplaySettingResponse>? RoomGroupMasterCategories { get; init; }
    public IEnumerable<CategoryOfRoomGroupDetailDisplaySettingResponse>? RoomGroupCategories { get; init; }
    public IEnumerable<CategoryOfRoomGroupDetailDisplaySettingResponse>? RoomGroupFeatureCategories { get; init; }
    public IEnumerable<CategoryOfRoomGroupDetailDisplaySettingResponse>? RoomGroupEquipmentCategories { get; init; }
    public IEnumerable<CategoryOfRoomGroupDetailDisplaySettingResponse>? RoomGroupAmenityCategories { get; init; }
    public string[]? Tags { get; set; }
}

public record CategoryOfRoomGroupDetailDisplaySettingResponse(
    long Id,
    string? Name,
    CategoryTypes Type
);
