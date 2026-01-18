namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupPrice;

public record RoomGroupPriceUpdatePriceCalendarCommand(
    long RoomTypeId,
    long SiteId
) : UpdateCommandBase<RoomGroupPriceUpdatePriceCalendarRequest, long>;
