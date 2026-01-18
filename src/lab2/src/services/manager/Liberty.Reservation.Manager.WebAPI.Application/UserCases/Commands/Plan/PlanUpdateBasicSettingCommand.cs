namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public record PlanUpdateBasicSettingCommand(
    long Id
) : UpdateCommandBase<PlanUpdateBasicSettingRequest, long>;
