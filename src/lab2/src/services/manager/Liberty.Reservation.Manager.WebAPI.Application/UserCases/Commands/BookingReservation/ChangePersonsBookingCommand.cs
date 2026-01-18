using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BookingReservation;

public record ChangePersonsBookingCommand(
    long Id
) : ActionCommandBase<BookingPriceRequest, BookingPriceResponse>;
