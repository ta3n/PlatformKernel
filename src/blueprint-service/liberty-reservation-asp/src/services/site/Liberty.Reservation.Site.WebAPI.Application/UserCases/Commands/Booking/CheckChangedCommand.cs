namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking;

public record CheckChangedCommand(
    long PlanId,
    long RoomGroupId
) : ActionCommandBase<CheckChangedRequest, bool>;
