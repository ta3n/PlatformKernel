using Liberty.Reservation.Application.Models.Requests;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

public interface IBookingCheckModifyInPriceService
{
    Task<bool> IsModifyInPriceAsync(
        ReservationEntity existingReservation,
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    );
}
