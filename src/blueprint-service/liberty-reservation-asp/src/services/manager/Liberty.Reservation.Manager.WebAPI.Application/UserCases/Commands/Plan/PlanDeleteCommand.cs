namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public record PlanDeleteCommand(
    long Id
) : DeleteCommandBase<string, long>;
