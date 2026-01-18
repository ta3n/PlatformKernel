using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.UseCases.Commands.BookingReservation;

public record BookingAbortCommand(
    Contexts.DataContexts.Entities.Data.Reservation ExistingReservation,
    ReservationStatus ReservationState,
    decimal CancellationPrice,
    float RateFee
) : UpdateCommandBase<BookingCancellationRequest, BookingAbortResponse>;
