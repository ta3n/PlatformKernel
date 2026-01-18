namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public record RoomGroupUpdateQuestionCommand(
    long Id
) : UpdateCommandBase<RoomGroupUpdateQuestionRequest, long>;
