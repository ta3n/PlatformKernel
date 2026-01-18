namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IBookingManagerModificationCheckerService
{
    bool CanCancelModify(
        long checkOutDate,
        bool isNoShow,
        ReservationStatus reservationState,
        TimeSpan? timeZone
    );

    bool CanBookingChangeModify(
        long checkOutDate,
        bool isNoShow,
        ReservationStatus reservationState,
        TimeSpan? timeZone
    );

    bool CanNoShowModify(
        TimeSpan checkInTime,
        long checkInDate,
        long checkOutDate,
        bool isNoShow,
        ReservationStatus reservationState,
        TimeSpan? timeZone
    );
}
