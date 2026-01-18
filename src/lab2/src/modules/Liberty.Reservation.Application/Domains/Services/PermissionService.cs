using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Application.Domains.Services;

public class PermissionService(
    ILogger<Permission> logger,
    IPermissionRepository permissionRepository
) : BaseService<Permission>(logger, permissionRepository, new PermissionNotfoundException()),
    IPermissionService
{
    public override Task<IEnumerable<Permission>> UpdateRangeAsync(
        IEnumerable<Permission> entitiesToUpdate,
        bool autoSave = true,
        Func<Permission, Permission, Permission>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.IsEnabled = updateEntity.IsEnabled;
                return existingEntity;
            });

        return base.UpdateRangeAsync(
            entitiesToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }
    public async Task<IEnumerable<PermissionResponse>> GetAllPermissionAsync(
        PermissionFilterParameter? filterParameter,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = permissionRepository
            .GetQueryableWithAsNoTracking();

        queryable = FilterQueryableByParameters(filterParameter, queryable);

        queryable = queryable.OrderBy(x => x.DisplayOrder);

        var settingItemConfigFlats = await queryable.ToDictionaryAsync(
                key => key.Code!,
                elementSelector => new PermissionResponse(
                    elementSelector.Key,
                    elementSelector.GroupName,
                    elementSelector.ParentCode,
                    elementSelector.ItemType,
                    elementSelector.Name,
                    elementSelector.DisplayOrder,
                    elementSelector.IsEnabled,
                    []
                ),
                cancellationToken
            )
            ?? throw new PermissionNotfoundException();

        var settingItemConfigs = new List<PermissionResponse>();

        foreach (var node in settingItemConfigFlats.Values)
        {
            if (string.IsNullOrEmpty(node.ParentCode) || !settingItemConfigFlats.TryGetValue(node.ParentCode, out var parent))
            {
                settingItemConfigs.Add(node);
                continue;
            }

            parent.Items!.Add(node);
        }
        AssignPath(settingItemConfigs);
        return settingItemConfigs;
    }

    private static void AssignPath(
        List<PermissionResponse> nodes,
        string? parentPath = null)
    {
        for (var i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];

            node.Path = parentPath is null
                ? $"[{i}]"
                : $"{parentPath}.items[{i}]";

            if (node.Items?.Count > 0)
            {
                AssignPath(node.Items, node.Path);
            }
        }
    }

    public async Task<IEnumerable<PermissionResponse>> GetAllPermissionFlatAsync(
        CancellationToken cancellationToken = default
    )
    {
        var queryable = permissionRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.ItemType == ItemTypes.Field)
            .OrderBy(x => x.DisplayOrder)
            .Select(
                x => new PermissionResponse(
                    x.Key,
                    x.GroupName,
                    x.ParentCode,
                    x.ItemType,
                    x.Name,
                    x.DisplayOrder,
                    x.IsEnabled,
                    null
                )
            );

        return await queryable.ToListAsync(cancellationToken);
    }

    public async Task<List<Permission>> GetPermissionByKeyAsync(
        string[] key,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = permissionRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => key.Contains(x.Key));
        return await queryable.ToListAsync(cancellationToken);
    }

    private static IQueryable<Permission> FilterQueryableByParameters(
        PermissionFilterParameter? filterParameter,
        IQueryable<Permission> queryable
    )
    {
        if (filterParameter?.GroupName != null)
        {
            queryable = queryable.Where(x => x.GroupName == filterParameter.GroupName);
        }

        if (filterParameter?.ParentCode != null)
        {
            queryable = queryable.Where(x => x.ParentCode == filterParameter.ParentCode);
        }

        if (filterParameter?.ItemTypes != null)
        {
            queryable = queryable.Where(x => x.ItemType == filterParameter.ItemTypes);
        }

        return queryable;
    }
}
