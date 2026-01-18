namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupPrice;

public record RoomGroupPriceUpdateDiscountCommand(
    long RoomTypeId,
    long SiteId
) : UpdateCommandBase<RoomGroupPriceUpdateDiscountRequest, long>;
