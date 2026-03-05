namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public record RoomGroupEnableCommand(
    long Id
) : UpdateCommandBase<RoomGroupEnabledRequest, long>;
