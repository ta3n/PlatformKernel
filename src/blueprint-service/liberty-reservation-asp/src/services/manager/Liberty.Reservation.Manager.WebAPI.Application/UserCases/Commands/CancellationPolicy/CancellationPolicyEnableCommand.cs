namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.CancellationPolicy;

public record CancellationPolicyEnableCommand(
    long Id
) : UpdateCommandBase<CancellationPolicyEnabledRequest, long>;
