namespace Liberty.Reservation.Application.Models;

public class RoomGroupFacilityDto
{
    public long Id { get; set; }
    public string GroupName { get; set; } = default!;
    public int BaseNumber { get; set; }
}
