namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupPrice;

public record RoomGroupPriceUpdateStandardPriceCommand(
    long RoomTypeId,
    long SiteId
) : UpdateCommandBase<RoomGroupPriceUpdateStandardPriceRequest, long>;
