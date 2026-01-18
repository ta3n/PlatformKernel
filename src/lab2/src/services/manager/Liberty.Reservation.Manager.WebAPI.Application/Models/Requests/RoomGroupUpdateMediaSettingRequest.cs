namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupUpdateMediaSettingRequest(
    IEnumerable<ImageOfRoomGroupUpdateMediaSettingRequest>? Images
)
{
    public long? Id { get; set; }
}

public record ImageOfRoomGroupUpdateMediaSettingRequest(
    long Id,
    int Index
);
