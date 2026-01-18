namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public record RoomGroupUpdateImportantNoteCommand(
    long Id
) : UpdateCommandBase<RoomGroupUpdateImportantNoteRequest, long>;
