namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public record PlanUpdateSaleCommand(
    long Id
) : UpdateCommandBase<PlanUpdateSaleRequest, long>;
