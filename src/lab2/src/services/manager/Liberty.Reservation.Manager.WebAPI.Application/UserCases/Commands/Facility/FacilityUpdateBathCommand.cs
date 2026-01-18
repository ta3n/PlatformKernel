namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;

public record FacilityUpdateBathCommand
    : UpdateCommandBase<FacilityUpdateBathRequest, long>;
