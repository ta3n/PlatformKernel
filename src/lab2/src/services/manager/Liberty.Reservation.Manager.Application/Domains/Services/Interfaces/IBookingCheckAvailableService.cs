using Liberty.Reservation.Application.Models.Requests;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IBookingCheckAvailableService
{
    Task<ReservationEntity> GetReservationAsync(
        long reservationId,
        CancellationToken cancellationToken = default
    );

    Task<bool> CheckAdjustAvailableAsync(
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    );

    Task<bool> CheckAvailableNumberOfNightsAsync(
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    );

    Task<bool> CheckAvailableNumberOfRoomsAsync(
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    );

    Task<bool> CheckAvailableReserverAsync(
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    );

    Task<bool> CheckAvailableMainUserAsync(
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    );

    Task<bool> CheckAvailableRoomPeoplesAsync(
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    );

    Task<bool> CheckAvailableNightOptionsAsync(
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    );

    Task<bool> CheckAvailableRoomRepresentativesAsync(
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    );
}
