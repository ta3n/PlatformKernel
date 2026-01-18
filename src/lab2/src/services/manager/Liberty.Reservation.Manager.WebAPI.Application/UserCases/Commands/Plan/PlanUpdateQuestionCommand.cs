namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public record PlanUpdateQuestionCommand(
    long Id
) : UpdateCommandBase<PlanUpdateQuestionRequest, long>;
