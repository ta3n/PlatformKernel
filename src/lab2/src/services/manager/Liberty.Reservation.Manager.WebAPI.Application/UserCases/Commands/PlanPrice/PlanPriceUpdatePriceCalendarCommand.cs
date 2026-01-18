namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

public record PlanPriceUpdatePriceCalendarCommand(
    long PlanId,
    long RoomTypeId,
    long SiteId
) : UpdateCommandBase<RoomGroupPriceUpdatePriceCalendarRequest, long>;
