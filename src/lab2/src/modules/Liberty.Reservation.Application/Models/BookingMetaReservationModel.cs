namespace Liberty.Reservation.Application.Models;

public record BookingMetaReservationModel(
    long ReservationId,
    long PlanId,
    long RoomGroupId,
    long BookingDateId,
    int RestIndex,
    int RoomGroupIndex,
    TimeSpan? CheckInTime,
    TimeSpan? CheckOutTime,
    bool BookingIsReserved
);
