namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

using ReservationEntity = Contexts.DataContexts.Entities.Data.Reservation;

public interface IBookingPaymentRestrictionService
{
    Task ValidateGlobalOnlinePaymentAsync(
        ReservationEntity existingReservation,
        bool isModifyInPrice,
        CancellationToken cancellationToken = default
    );
}
