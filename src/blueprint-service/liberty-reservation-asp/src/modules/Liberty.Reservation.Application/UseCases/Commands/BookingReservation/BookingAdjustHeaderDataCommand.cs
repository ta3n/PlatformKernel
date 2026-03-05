using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Models.Requests;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Application.UseCases.Commands.BookingReservation;

public record BookingAdjustHeaderDataCommand(
    ReservationStatus ReservationState
) : UpdateCommandBase<BookingAdjustRequest, ReservationEntity>;
