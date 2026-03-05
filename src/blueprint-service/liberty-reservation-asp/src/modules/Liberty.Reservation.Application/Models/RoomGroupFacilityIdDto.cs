namespace Liberty.Reservation.Application.Models;

public class RoomGroupFacilityIdDto
{
    public long RoomGroupId { get; set; }
    public long FacilityId { get; set; }
    public required string RoomId { get; set; }
    public required string HotelId { get; set; }
}
