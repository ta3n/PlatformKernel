namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;

public record FacilityUpdatePaymentMethodCommand
    : UpdateCommandBase<FacilityUpdatePaymentMethodRequest, long>;
