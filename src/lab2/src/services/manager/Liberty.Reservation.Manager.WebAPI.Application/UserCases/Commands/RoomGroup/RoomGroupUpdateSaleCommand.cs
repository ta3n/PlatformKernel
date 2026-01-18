namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public record RoomGroupUpdateSaleCommand(
    long Id
) : UpdateCommandBase<RoomGroupUpdateSaleRequest, long>;
