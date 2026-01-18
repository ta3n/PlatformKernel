using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Permission;

public class PermissionCreateCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPermissionService permissionService
) : CreateCommandHandlerBase<PermissionCreateCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PermissionCreateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var permission = Mapper.Map<Reservation.Application.Contexts.DataContexts.Entities.Data.Permission>(payload);
        permission.IsEnabled = permission.ItemType == ItemTypes.Field;
        var response = await permissionService.CreateAsync(permission, cancellationToken: cancellationToken);
        return response.Id;
    }
}
