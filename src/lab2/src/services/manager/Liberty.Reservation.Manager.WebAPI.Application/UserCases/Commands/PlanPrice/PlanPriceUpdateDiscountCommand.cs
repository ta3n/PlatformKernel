namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

public record PlanPriceUpdateDiscountCommand(
    long PlanId,
    long RoomTypeId,
    long SiteId
) : UpdateCommandBase<RoomGroupPriceUpdateDiscountRequest, long>;
