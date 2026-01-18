using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Booking.Worker.Application.Models;

public class PayloadOfRequestCreate
{
    public long PlanId { get; set; }
    public long RoomGroupId { get; set; }
    public SiteBookingCreateRequest? Payload { get; init; }
}

public class PayloadOfRequestAdjust
{
    public BookingAdjustRequest? Payload { get; init; }
}
