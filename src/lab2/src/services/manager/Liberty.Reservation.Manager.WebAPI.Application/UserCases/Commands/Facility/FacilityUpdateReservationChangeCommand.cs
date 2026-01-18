namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;

public record FacilityUpdateReservationChangeCommand
    : UpdateCommandBase<FacilityUpdateReservationChangeRequest, long>;
