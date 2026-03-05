using Liberty.Reservation.Application.Cqrs.BaseCommands;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.AlertMessage;

public record AlertMessageUpdateCommand : UpdateCommandBase<AlertMessageUpdateRequest, long>;
