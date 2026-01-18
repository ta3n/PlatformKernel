namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;

public record FacilityUpdateAcceptCommand
    : UpdateCommandBase<FacilityUpdateAcceptRequest, long>;
