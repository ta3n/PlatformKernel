namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record FacilityUpdateBasicSettingRequest(
    string? Description,
    string? Fax,
    string? Url,
    string? Name,
    string? Kana,
    string? Postcode,
    string? Address1,
    string? Address2,
    string? Address3,
    string? Address4,
    string? Phone,
    int? RoomNumberWesternStyle,
    int? RoomNumberJapaneseStyle,
    int? RoomNumberJapaneseWesternStyle,
    int? RoomNumberOtherStyle,
    long? AreaId,
    long? FacilityTypeId,
    FileOfFacilityRequest? File
);

public record FileOfFacilityRequest(
    string? Code
);
