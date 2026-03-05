namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;

public record FacilityUpdateMinimumPriceCommand
    : UpdateCommandBase<FacilityUpdateMinimumPriceRequest, long>;
