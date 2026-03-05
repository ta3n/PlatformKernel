namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

public record PlanPriceUpdateChildrenPriceCommand(
    long PlanId,
    long RoomTypeId,
    long SiteId
) : UpdateCommandBase<RoomGroupPriceUpdateChildrenPriceRequest, long>;
