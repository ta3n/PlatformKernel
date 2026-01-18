using Liberty.Reservation.Application.Cqrs.BaseCommands;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Area;

public record AreaEnableCommand : UpdateCommandBase<AreaEnabledRequest, long>;
