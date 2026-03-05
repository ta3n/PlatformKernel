using Liberty.ApplicationShared.Utils;
using Liberty.Cache.Services;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class RoomGroupService(
    ILogger<RoomGroup> logger,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IRoomGroupRepository roomGroupRepository,
    IFacilityRoomGroupRepository facilityRoomGroupRepository,
    IRoomGroupCategoryService roomGroupCategoryService,
    IRoomGroupSiteService roomGroupSiteService,
    IRoomGroupBedTypeService roomGroupBedTypeService
) : BaseService<RoomGroup>(logger, cacheService, roomGroupRepository, new RoomGroupNotfoundException()),
    IRoomGroupService
{
    public override Task<RoomGroup> CreateAsync(
        RoomGroup entityToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        entityToCreate.Code = EntityUtil.CreateCode();

        return base.CreateAsync(entityToCreate, autoSave, cancellationToken);
    }

    public override Task<RoomGroup> UpdateAsync(
        RoomGroup entityToUpdate,
        bool autoSave = true,
        Func<RoomGroup, RoomGroup, RoomGroup>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.Name = updateEntity.Name;
                existingEntity.Description = updateEntity.Description;
                existingEntity.BaseNumber = updateEntity.BaseNumber;
                existingEntity.CapacityMin = updateEntity.CapacityMin;
                existingEntity.CapacityMax = updateEntity.CapacityMax;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public Task<RoomGroup> UpdateBasicConfigurationOfRoomGroupAsync(
        RoomGroup entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            (
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.Name ??= [];
                existingEntity.Name?.UpdateLocalized(updateEntity.Name);
                existingEntity.GroupName = updateEntity.GroupName;
                existingEntity.Overview ??= [];
                existingEntity.Overview?.UpdateLocalized(updateEntity.Overview);
                existingEntity.Description ??= [];
                existingEntity.Description?.UpdateLocalized(updateEntity.Description);
                existingEntity.BaseNumber = updateEntity.BaseNumber;
                existingEntity.CapacityMin = updateEntity.CapacityMin;
                existingEntity.CapacityMax = updateEntity.CapacityMax;
                existingEntity.Size = updateEntity.Size;
                existingEntity.RoomGroupSizeUnitType = updateEntity.RoomGroupSizeUnitType;
                existingEntity.IsEnabledSmoking = updateEntity.IsEnabledSmoking;
                existingEntity.MetaJson = updateEntity.MetaJson;
                existingEntity.IsDescriptionVisible = updateEntity.IsDescriptionVisible;
                existingEntity.IsOverviewVisible = updateEntity.IsOverviewVisible;
                existingEntity.IsRoomSizeVisible = updateEntity.IsRoomSizeVisible;
                existingEntity.IsBedTypeVisible = updateEntity.IsBedTypeVisible;

                return existingEntity;
            },
            cancellationToken
        );
    }

    public Task<RoomGroup> UpdateDisplaySettingOfRoomGroupAsync(
        RoomGroup entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            (
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.Tag = updateEntity.Tag;

                return existingEntity;
            },
            cancellationToken
        );
    }

    public async Task<RoomGroup> CreateRoomGroupWithFacilityAsync(
        RoomGroup entityToCreate,
        long facilityId,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var roomGroup = await CreateAsync(entityToCreate, autoSave, cancellationToken);

        var newFacilityRoomGroup = new FacilityRoomGroup
        {
            FacilityId = facilityId,
            RoomGroup = roomGroup,
            IsEnabled = true
        };

        await facilityRoomGroupRepository.AddAsync(newFacilityRoomGroup, autoSave, cancellationToken);

        return roomGroup;
    }

    public async Task<(IEnumerable<RoomGroupCategory> adds, IEnumerable<RoomGroupCategory> removes)>
        ChangeCategoriesOfRoomGroupAsync(
            long roomGroupId,
            IEnumerable<long> categoryIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var (adds, removes) = await roomGroupCategoryService.ChangeCategoriesOfRoomGroupAsync(
            roomGroupId,
            categoryIds,
            autoSave,
            cancellationToken
        );

        return (adds, removes);
    }

    public async Task<(IEnumerable<RoomGroupSite> adds, IEnumerable<RoomGroupSite> removes)>
        ChangeSitesOfRoomGroupAsync(
            long roomGroupId,
            IEnumerable<long> siteIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var (adds, removes) = await roomGroupSiteService.ChangeSitesOfRoomGroupAsync(
            roomGroupId,
            siteIds,
            autoSave,
            cancellationToken
        );

        return (adds, removes);
    }

    public async Task<(IEnumerable<RoomGroupBedType> adds, IEnumerable<RoomGroupBedType> removes)>
        ChangeBedTypesOfRoomGroupAsync(
            long roomGroupId,
            IEnumerable<(long bedTypeId, int number)> adjustBedTypes,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var (adds, removes) = await roomGroupBedTypeService.ChangeBedTypesOfRoomGroupAsync(
            roomGroupId,
            adjustBedTypes,
            autoSave,
            cancellationToken
        );

        return (adds, removes);
    }

    public async Task<bool> CheckRoomGroupNameExistAsync(
        long roomGroupId,
        string? groupName,
        CancellationToken cancellationToken = default
    )
    {
        return await roomGroupRepository.GetQueryableWithAsNoTracking()
            .AnyAsync(
                x => x.GroupName == groupName
                    && x.Id == roomGroupId,
                cancellationToken
            );
    }

    protected override string GetCacheKey(
        string methodName = "",
        params string[] keys
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;
        return $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}:" + base.GetCacheKey(methodName, keys);
    }

    protected override IQueryable<RoomGroup> GetQueryable()
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return roomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.FacilityRoomGroups!.Any(
                    t => t.FacilityId == facilityId
                )
            );
    }

    public async Task UpdateLastModifiedAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        var longDate = ConvertUtil.ToLong(
            ConvertUtil.ToString(DateTime.UtcNow, "yyyyMMddHHmmssfff")
        );

        await roomGroupRepository
            .GetQueryable()
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(b => b.UpdatedAt, longDate),
                cancellationToken
            );
    }

    public async Task<long> GetRoomGroupByGroupNameAsync(
        string groupName,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = roomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.GroupName == groupName)
            .Where(x => x.IsEnabled)
            .Select(x => x.Id);
        var roomGroupId = await queryable.SingleOrDefaultAsync(cancellationToken);

        return roomGroupId;
    }
}
