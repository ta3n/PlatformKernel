namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupPrice;

public record RoomGroupPriceUpdateChildrenPriceCommand(
    long RoomTypeId,
    long SiteId
) : UpdateCommandBase<RoomGroupPriceUpdateChildrenPriceRequest, long>;
