namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public record PlanUpdateMealCommand(
    long Id
) : UpdateCommandBase<PlanUpdateMealRequest, long>;
