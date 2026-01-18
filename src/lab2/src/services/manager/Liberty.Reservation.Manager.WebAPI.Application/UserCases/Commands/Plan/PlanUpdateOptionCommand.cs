namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public record PlanUpdateOptionCommand(
    long Id
) : UpdateCommandBase<PlanUpdateOptionRequest, long>;
