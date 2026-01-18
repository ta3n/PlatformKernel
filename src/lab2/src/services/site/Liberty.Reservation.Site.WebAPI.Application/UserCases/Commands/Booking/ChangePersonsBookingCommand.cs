using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking;

public record ChangePersonsBookingCommand(
    long PlanId,
    long RoomGroupId
) : ActionCommandBase<BookingPriceRequest, BookingPriceResponse>;
