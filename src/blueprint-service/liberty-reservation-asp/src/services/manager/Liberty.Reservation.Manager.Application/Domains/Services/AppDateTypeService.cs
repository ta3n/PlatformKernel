using Liberty.ApplicationShared.Utils;
using Liberty.Cache.Services;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class AppDateTypeService(
    ILogger<AppDateTypeService> logger,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IAppDateTypeRepository appDateTypeRepository
) : BaseService<AppDateType>(logger, cacheService, appDateTypeRepository, new AppDateTypeNotfoundException()),
    IAppDateTypeService
{
    protected override string GetCacheKey(
        string methodName = "",
        params string[] keys
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;
        return $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}:" + base.GetCacheKey(methodName, keys);
    }

    protected override IQueryable<AppDateType> GetQueryable()
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return appDateTypeRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.FacilityAppDateTypes!.Any(
                    t => t.FacilityId == facilityId
                )
            );
    }

    public override Task<AppDateType> CreateAsync(
        AppDateType entityToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        entityToCreate.Code = EntityUtil.CreateCode();

        return base.CreateAsync(entityToCreate, autoSave, cancellationToken);
    }

    public override Task<AppDateType> UpdateAsync(
        AppDateType entityToUpdate,
        bool autoSave = true,
        Func<AppDateType, AppDateType, AppDateType>? updateAction = null,
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
                existingEntity.ShortName = updateEntity.ShortName;
                existingEntity.Description = updateEntity.Description;
                existingEntity.Color = updateEntity.Color;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }
}
