using Liberty.Reservation.Application.Models.Requests;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IBookingCreateService
{
    Task<ReservationEntity> CreateBookingAsync(
        BookingCreateRequest bookingCreateRequest,
        ReservationEntity existingReservation,
        CancellationToken cancellationToken = default
    );
}
