namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public record PlanUpdatePaymentMethodCommand(
    long Id
) : UpdateCommandBase<PlanUpdatePaymentMethodRequest, long>;
