using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking;

public record BookingCreateCommand(
    long PlanId,
    long RoomGroupId
) : CreateCommandBase<SiteBookingCreateRequest, string>;
