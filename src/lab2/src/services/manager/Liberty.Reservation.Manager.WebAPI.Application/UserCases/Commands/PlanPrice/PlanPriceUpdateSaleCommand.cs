namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

public record PlanPriceUpdateSaleCommand(
    long PlanId,
    long RoomTypeId,
    long SiteId
) : UpdateCommandBase<RoomGroupPriceUpdateSaleRequest, long>;
