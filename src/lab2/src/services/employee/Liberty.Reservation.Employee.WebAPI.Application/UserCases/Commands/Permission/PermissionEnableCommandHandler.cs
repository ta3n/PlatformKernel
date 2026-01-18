using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Models;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Permission;

public class PermissionEnableCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPermissionService permissionService
    ) : UpdateCommandHandlerBase<PermissionEnableCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PermissionEnableCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var flatsPermission = Flatten(payload);
        var permissionKey = flatsPermission
            .Select(x => x.Key)
            .OfType<string>()
            .ToArray();

        var permissionsByKey = await permissionService.GetPermissionByKeyAsync(
            permissionKey,
            cancellationToken
        );

        var permissionLookup = flatsPermission.ToDictionary(x => x.Key!, StringComparer.OrdinalIgnoreCase);

        foreach (var permissionItem in permissionsByKey)
        {
            if (permissionLookup.TryGetValue(permissionItem.Key!, out var permission))
            {
                permissionItem.IsEnabled = permission.IsEnabled;
            }
        }

        await permissionService.UpdateRangeAsync(permissionsByKey,  cancellationToken:cancellationToken);

        return permissionsByKey.Count;
    }

    private static List<PermissionFlatDto> Flatten(
        IEnumerable<PermissionEnableRequest> roots)
    {
        var result = new List<PermissionFlatDto>();
        foreach (var root in roots)
        {
            Traverse(root, null, result);
        }

        return result;
    }

    private static void Traverse(
        PermissionEnableRequest node,
        string? parentKey,
        List<PermissionFlatDto> acc)
    {
        acc.Add(new PermissionFlatDto(node.Key, parentKey, node.IsEnabled));

        foreach (var child in node.Items?.OfType<PermissionEnableRequest>() ?? [])
        {
            Traverse(child, child.ParentCode, acc);
        }
    }
}
