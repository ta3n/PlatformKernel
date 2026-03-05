namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record FacilityDetailBasicSettingResponse(
    string? Code,
    string? Name,
    string? Kana,
    string? Description,
    string? PostCode,
    string? Address1,
    string? Address2,
    string? Address3,
    string? Address4,
    string? Phone,
    string? Fax,
    string? Url,
    long? AreaId,
    long? FacilityTypeId,
    int? RoomNumberWesternStyle,
    int? RoomNumberJapaneseStyle,
    int? RoomNumberJapaneseWesternStyle,
    int? RoomNumberOtherStyle,
    FileOfFacilityResponse? File
);

public record FileOfFacilityResponse(
    string? Code
)
{
    public string? Description { get; set; }
};
