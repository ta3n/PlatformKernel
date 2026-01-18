using Liberty.Reservation.Application.Cqrs.BaseCommands;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Permission;

public record PermissionCreateCommand : CreateCommandBase<PermissionCreateRequest, long>;
