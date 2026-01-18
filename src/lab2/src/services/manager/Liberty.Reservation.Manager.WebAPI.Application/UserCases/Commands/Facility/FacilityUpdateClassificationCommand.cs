namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;

public record FacilityUpdateClassificationCommand
    : UpdateCommandBase<FacilityUpdateClassificationRequest, long>;
