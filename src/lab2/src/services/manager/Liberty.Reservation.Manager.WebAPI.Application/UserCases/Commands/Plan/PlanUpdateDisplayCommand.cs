namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public record PlanUpdateDisplayCommand(
    long Id
) : UpdateCommandBase<PlanUpdateDisplayRequest, long>;
