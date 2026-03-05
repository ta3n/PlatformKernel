namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupUpdateDisplaySettingRequest(
    long[]? RoomGroupMasterCategoryIds, // Room Classification
    long[]? RoomGroupCategoryIds, // Room Categories
    long[]? RoomGroupFeatureCategoryIds, // Room Features
    long[]? RoomGroupEquipmentCategoryIds, // Room Equipment
    long[]? RoomAmenityCategoryIds, // Room Amenities
    string[]? Tags
)
{
    public long? Id { get; set; }
}
