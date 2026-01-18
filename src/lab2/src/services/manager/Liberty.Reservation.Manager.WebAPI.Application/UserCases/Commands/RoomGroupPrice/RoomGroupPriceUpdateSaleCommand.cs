namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupPrice;

public record RoomGroupPriceUpdateSaleCommand(
    long RoomTypeId,
    long SiteId
) : UpdateCommandBase<RoomGroupPriceUpdateSaleRequest, long>;
