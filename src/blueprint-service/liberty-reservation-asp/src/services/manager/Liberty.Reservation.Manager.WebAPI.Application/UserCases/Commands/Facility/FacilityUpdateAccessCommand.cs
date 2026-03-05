namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;

public record FacilityUpdateAccessCommand
    : UpdateCommandBase<FacilityUpdateAccessRequest, long>;
