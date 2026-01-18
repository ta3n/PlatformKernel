namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record RoomGroupAppDateResponse(
    long AppDateId,
    long RoomGroupId,
    string RoomGroupName,
    int SellNumber,
    bool IsNotSold,
    int BaseNumber,
    string? GroupName
)
{
    public int RemainNumber { get; set; }
    public int ReservedNumber { get; set; }
}
