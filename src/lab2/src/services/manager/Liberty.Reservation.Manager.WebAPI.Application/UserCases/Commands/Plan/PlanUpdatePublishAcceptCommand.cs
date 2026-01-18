namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public record PlanUpdatePublishAcceptCommand(
    long Id
) : UpdateCommandBase<PlanUpdatePublishAcceptRequest, long>;
