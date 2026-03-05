using Newtonsoft.Json;

namespace Liberty.Reservation.Site.WebAPI.Application.Models.Responses;

public record RoomGroupResponse(
    long Id,
    string? Code,
    string? Name,
    string? Tag,
    string? Description,
    string? Overview,
    int CapacityMax,
    int CapacityMin,
    float? Size,
    bool IsEnabledSmoking,
    bool IsRoomSizeVisible,
    bool IsBedTypeVisible,
    RoomGroupSizeUnitTypes? RoomGroupSizeUnitType,
    IEnumerable<string>? BedTypes,
    [property: JsonIgnore] IEnumerable<string>? MasterCategories,
    [property: JsonIgnore] IEnumerable<string>? FacilityCategories,
    [property: JsonIgnore] IEnumerable<string>? MasterAmenities,
    [property: JsonIgnore] IEnumerable<string>? FacilityAmenities,
    IEnumerable<FileOfBookingResponse>? Files,
    long? LastUpdatedAt
)
{
    public IEnumerable<string> Categories { get; init; } =
        [.. MasterCategories ?? [], .. FacilityCategories ?? []];

    public IEnumerable<string> Amenities { get; init; } =
        [.. MasterAmenities ?? [], .. FacilityAmenities ?? []];
}
