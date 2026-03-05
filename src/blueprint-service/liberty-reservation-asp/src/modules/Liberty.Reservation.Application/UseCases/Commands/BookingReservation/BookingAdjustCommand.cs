using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Application.UseCases.Commands.BookingReservation;

public record BookingAdjustCommand(
    Contexts.DataContexts.Entities.Data.Reservation ExistingReservation,
    ReservationStatus ReservationState,
    bool IsModifyInPrice
) : UpdateCommandBase<BookingAdjustRequest, Contexts.DataContexts.Entities.Data.Reservation>
{
    public bool IsNotCheckValidDateLimit { get; set; } = false;
}
