namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public record PlanEnabledCommand(
    long Id
) : UpdateCommandBase<PlanEnabledRequest, long>;
