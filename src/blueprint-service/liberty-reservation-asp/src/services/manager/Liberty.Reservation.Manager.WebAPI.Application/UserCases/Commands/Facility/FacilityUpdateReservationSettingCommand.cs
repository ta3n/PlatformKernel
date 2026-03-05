namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;

public record FacilityUpdateReservationSettingCommand
    : UpdateCommandBase<FacilityUpdateReservationSettingRequest, long>;
