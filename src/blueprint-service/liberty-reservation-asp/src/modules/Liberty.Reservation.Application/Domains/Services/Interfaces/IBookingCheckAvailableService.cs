using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

public interface IBookingCheckAvailableService
{
    Task<ReservationEntity> GetReservationByUserAsync(
        long reservationId,
        string? userCode,
        CancellationToken cancellationToken = default
    );

    Task<ReservationBasicModel> GetReservationBasicByUserAsync(
        long reservationId,
        string? userCode,
        CancellationToken cancellationToken = default
    );

    Task<ReservationEntity> GetReservationByFacilityAsync(
        long reservationId,
        long facilityId,
        CancellationToken cancellationToken = default
    );

    Task<ReservationBasicModel> GetReservationBasicByFacilityAsync(
        long reservationId,
        long facilityId,
        CancellationToken cancellationToken = default
    );

    Task<bool> CheckAdjustAvailableAsync(
        long facilityId,
        long planId,
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    );

    Task<bool> CheckAvailableReserverAsync(
        long facilityId,
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    );

    Task<bool> CheckAvailableMainUserAsync(
        long facilityId,
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    );

    Task<bool> CheckAvailableRoomPeoplesAsync(
        long facilityId,
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    );

    Task<bool> CheckAvailableNightOptionsAsync(
        long facilityId,
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    );

    Task<bool> CheckAvailableRoomRepresentativesAsync(
        long facilityId,
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    );

    Task<bool> IsNightNumberAsync(
        long planId,
        long roomGroupId,
        long siteId,
        long checkInDate,
        long restNumber,
        CancellationToken cancellationToken = default
    );

    Task<bool> IsRoomNumberAsync(
        long planId,
        long roomGroupId,
        long checkInDate,
        long roomNumber,
        long restNumber,
        long reservationId = 0,
        CancellationToken cancellationToken = default
    );

    Task<bool> IsReceptionAvailableAsync(
        long planId,
        long checkInDate,
        CancellationToken cancellationToken
    );

    Task<(ReservationEntity?, bool)> GetReservationByOrderIdAsync(
        string orderId,
        CancellationToken cancellationToken
    );

    Task<ReservationEntity> GetReservationByCodeAsync(
        string code,
        string? userCode,
        CancellationToken cancellationToken = default
    );

    Task<bool> IsInvalidOptionItemsAsync(
        BookingPriceRequest bookingPriceRequest,
        long facilityId,
        long reservationId = 0,
        CancellationToken cancellationToken = default
    );

    Task<bool> IsInvalidChangePersonsAsync(
        BookingPriceRequest bookingPriceRequest,
        BookingPlanModel planModel,
        long facilityId,
        long roomGroupId,
        long siteId,
        bool useDailyPerson,
        CancellationToken cancellationToken
    );
}
