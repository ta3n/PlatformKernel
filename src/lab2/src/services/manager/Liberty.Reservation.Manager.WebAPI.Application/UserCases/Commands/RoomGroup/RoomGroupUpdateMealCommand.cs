namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public record RoomGroupUpdateMealCommand(
    long Id
) : UpdateCommandBase<RoomGroupUpdateMealRequest, long>;
