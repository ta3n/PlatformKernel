using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Models;

public class ReservationPlanRoomGroupAppDateDto
{
    public long RoomGroupId { get; set; }
    public long BookingDateId { get; set; }
    public ReservationStatus ReservationState { get; set; }
}
