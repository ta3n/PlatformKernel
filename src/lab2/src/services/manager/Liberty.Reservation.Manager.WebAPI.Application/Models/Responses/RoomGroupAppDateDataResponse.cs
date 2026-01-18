namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public class RoomGroupAppDateDataResponse
{
    public long AppDateId { get; set; }
    public long RoomGroupId { get; set; }
    public string RoomGroupName { get; set; } = string.Empty;
    public int SellNumber { get; set; }
    public bool IsNotSold { get; set; }
    public int ReservedNumber { get; set; }
    public int RemainNumber { get; set; }
    public int BaseNumber { get; set; }
    public string? GroupName { get; set; }
}
