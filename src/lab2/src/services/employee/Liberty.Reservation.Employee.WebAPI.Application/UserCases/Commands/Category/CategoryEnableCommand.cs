using Liberty.Reservation.Application.Cqrs.BaseCommands;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Category;

public record CategoryEnableCommand : UpdateCommandBase<CategoryEnabledRequest, long>;
