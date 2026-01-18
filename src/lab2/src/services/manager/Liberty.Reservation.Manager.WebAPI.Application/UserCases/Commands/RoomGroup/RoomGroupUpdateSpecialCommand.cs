namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public record RoomGroupUpdateSpecialCommand(
    long Id
) : UpdateCommandBase<RoomGroupUpdateSpecialRequest, long>;
