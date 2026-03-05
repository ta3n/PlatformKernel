using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Models;

public record BookingHoldCheckModel(
    long FacilityId,
    long SiteId,
    long PlanId,
    long RoomId,
    long CheckInDate,
    int NumberOfNights,
    int NumberOfRooms,
    string? UserCode,
    string BookingTempCode,
    int HoldTimeInSeconds
)
{
    public DateTime CheckInDateTime => AppDate.GetDateTime(CheckInDate);

    public DateTime CheckOutDateTime => CheckInDateTime.AddDays(NumberOfNights - 1);

    public long CheckOutDate => AppDate.GetId(CheckOutDateTime);
}
