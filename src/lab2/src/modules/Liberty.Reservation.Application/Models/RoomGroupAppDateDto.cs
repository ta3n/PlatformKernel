namespace Liberty.Reservation.Application.Models;

public class RoomGroupAppDateDto
{
    public int RoomGroupId { get; set; }
    public long AppDateId { get; set; }
    public bool IsNotSelled { get; set; }
    public int SellNumber { get; set; }
    public int BaseNumber { get; set; }
}
