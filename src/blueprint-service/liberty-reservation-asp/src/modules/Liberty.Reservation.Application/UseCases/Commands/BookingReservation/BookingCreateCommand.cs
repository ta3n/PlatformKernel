using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Models.Requests;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Application.UseCases.Commands.BookingReservation;

public record BookingCreateCommand(
    BookingExternalInfoRequest ExternalInfo
) : CreateCommandBase<BookingCreateRequest, ReservationEntity>;
