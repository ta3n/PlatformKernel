namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record RoomGroupDetailMediaSettingResponse(
    long Id,
    IEnumerable<ImageOfRoomGroupDetailMediaSettingResponse> Images
);

public record ImageOfRoomGroupDetailMediaSettingResponse(
    long Id,
    string Code,
    int Index,
    bool IsEnabled,
    string? Description
);
