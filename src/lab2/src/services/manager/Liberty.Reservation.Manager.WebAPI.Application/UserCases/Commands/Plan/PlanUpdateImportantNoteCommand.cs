namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public record PlanUpdateImportantNoteCommand(
    long Id
) : UpdateCommandBase<PlanUpdateImportantNoteRequest, long>;
