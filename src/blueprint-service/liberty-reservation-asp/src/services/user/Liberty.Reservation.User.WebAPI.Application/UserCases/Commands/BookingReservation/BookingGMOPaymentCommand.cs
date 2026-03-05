using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.User.WebAPI.Application.Models.Requests;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;

public record BookingGmoPaymentCommand : UpdateCommandBase<GmoPaymentLinkPlusRequest, string>;
