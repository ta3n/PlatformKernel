namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public record PlanUpdateCancelCommand(
    long Id
) : UpdateCommandBase<PlanUpdateCancelRequest, long>;
