using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingPaymentRestrictionService(
    IBookingSystemConfigService bookingSystemConfigService,
    IBookingReservationService bookingReservationService
) : IBookingPaymentRestrictionService
{
    public async Task ValidateGlobalOnlinePaymentAsync(
        ReservationEntity existingReservation,
        bool isModifyInPrice,
        CancellationToken cancellationToken = default
    )
    {
        var canOnlinePayment = await bookingSystemConfigService.GetSystemConfigOnlinePaymenAsync(cancellationToken);

        if (isModifyInPrice && !canOnlinePayment && existingReservation.IsOnlinePayment)
        {
            throw new GlobalCanOnlinePaymentNotAllowException();
        }

        var isOnlyOnlinePaymentFacility = await bookingReservationService
            .IsBookingHasFacilitySetOnlyOnlinePaymentMethodAsync(existingReservation.Id, cancellationToken);

        if (existingReservation.IsOnlinePayment && isOnlyOnlinePaymentFacility && isModifyInPrice)
        {
            throw new ReservationOnlineBookingModificationRestrictionException();
        }
    }
}
