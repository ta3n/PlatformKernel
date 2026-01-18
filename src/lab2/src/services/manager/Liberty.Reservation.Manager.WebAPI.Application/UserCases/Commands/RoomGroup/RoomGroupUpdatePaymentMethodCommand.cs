namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public record RoomGroupUpdatePaymentMethodCommand(
    long Id
) : UpdateCommandBase<RoomGroupUpdatePaymentMethodRequest, long>;
