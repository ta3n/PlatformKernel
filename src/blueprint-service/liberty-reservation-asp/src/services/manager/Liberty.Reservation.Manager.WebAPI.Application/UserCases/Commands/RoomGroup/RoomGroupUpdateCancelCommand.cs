namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public record RoomGroupUpdateCancelCommand(
    long Id
) : UpdateCommandBase<RoomGroupUpdateCancelRequest, long>;
