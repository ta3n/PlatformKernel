namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IBookingManagerModificationCheckerService
{
    bool CanCancelModify(
        long checkOutDate,
        bool isNoShow,
        ReservationStatus reservationState
    );

    bool CanBookingChangeModify(
        long checkInDate,
        bool isNoShow,
        ReservationStatus reservationState
    );

    bool CanNoShowModify(
        TimeSpan checkInTime,
        long checkInDate,
        long checkOutDate,
        bool isNoShow,
        ReservationStatus reservationState
    );
}
