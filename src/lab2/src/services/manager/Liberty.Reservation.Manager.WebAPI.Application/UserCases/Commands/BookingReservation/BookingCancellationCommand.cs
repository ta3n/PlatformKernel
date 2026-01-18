using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BookingReservation;

public record BookingCancellationCommand : UpdateCommandBase<BookingCancellationByManagerRequest, long>;
