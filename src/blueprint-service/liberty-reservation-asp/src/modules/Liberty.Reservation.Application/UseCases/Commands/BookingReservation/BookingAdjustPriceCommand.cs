using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Models.Requests;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Application.UseCases.Commands.BookingReservation;

public record BookingAdjustPriceCommand(
    ReservationEntity ExistingReservation,
    ReservationStatus ReservationState
) : CreateCommandBase<BookingAdjustRequest, ReservationEntity>
{
    public bool IsNotCheckValidDateLimit { get; set; } = false;
}
