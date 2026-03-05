namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public record PlanEnabledRoomTypeCommand(
    long PlanId,
    long RomTypeId
) : UpdateCommandBase<PlanEnabledRoomTypeRequest, long>;
