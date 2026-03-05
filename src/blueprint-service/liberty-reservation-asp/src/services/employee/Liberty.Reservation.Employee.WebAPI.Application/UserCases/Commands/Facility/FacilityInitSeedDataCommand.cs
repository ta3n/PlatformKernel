using Liberty.Reservation.Application.Cqrs.BaseCommands;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Facility;

public record FacilityInitSeedDataCommand : UpdateCommandBase<FacilityInitSeedDataRequest, long>;
