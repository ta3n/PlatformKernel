namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public record PlanUpdateSpecialCommand(
    long Id
) : UpdateCommandBase<PlanUpdateSpecialRequest, long>;
