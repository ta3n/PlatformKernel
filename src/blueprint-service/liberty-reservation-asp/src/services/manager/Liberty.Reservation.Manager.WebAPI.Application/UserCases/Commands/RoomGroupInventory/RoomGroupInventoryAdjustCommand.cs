namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupInventory;

public record RoomGroupInventoryAdjustCommand
    : UpdateCommandBase<IEnumerable<RoomGroupChangeRemainRequest>, (long[] addAppDateIds, long[] editAppDateIds)>;
