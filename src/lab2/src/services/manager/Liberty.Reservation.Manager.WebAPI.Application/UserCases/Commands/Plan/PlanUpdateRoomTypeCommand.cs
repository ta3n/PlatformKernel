namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public record PlanUpdateRoomTypeCommand(
    long Id
) : UpdateCommandBase<PlanUpdateRoomTypeRequest, long>;
