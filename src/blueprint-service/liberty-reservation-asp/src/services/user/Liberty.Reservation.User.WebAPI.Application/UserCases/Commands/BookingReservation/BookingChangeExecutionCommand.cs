using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;

public record BookingChangeExecutionCommand : UpdateCommandBase<BookingAdjustRequest, long>;
