namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

public class RoomGroupAppDateResponse
{
    public long AppDateId { get; set; }
    public string? RoomGroupId { get; set; }
    public string? RoomGroupName { get; set; }
    public int SellNumber { get; set; }
    public bool IsNotSold { get; set; }
    public int RemainNumber { get; set; }
    public int ReservedNumber { get; set; }
}
