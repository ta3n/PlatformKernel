namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public record RoomGroupUpdateOptionCommand(
    long Id
) : UpdateCommandBase<RoomGroupUpdateOptionRequest, long>;
