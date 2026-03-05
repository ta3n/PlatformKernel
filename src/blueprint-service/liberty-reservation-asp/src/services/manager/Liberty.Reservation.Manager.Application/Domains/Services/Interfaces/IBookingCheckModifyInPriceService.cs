using Liberty.Reservation.Application.Models.Requests;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IBookingCheckModifyInPriceService
{
    Task<bool> IsModifyInPriceAsync(
        ReservationEntity existingReservation,
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    );

    Task<bool> IsModifyPeoplesAsync(
        ReservationEntity existingReservation,
        List<NightPeopleOfReservationAdjustRequest>? roomPeoplesRequest,
        CancellationToken cancellationToken = default
    );

    Task<bool> IsModifyOptionsAsync(
        ReservationEntity existingReservation,
        List<NightOptionOfReservationAdjustRequest>? nightOptionsRequest,
        CancellationToken cancellationToken = default
    );
}
