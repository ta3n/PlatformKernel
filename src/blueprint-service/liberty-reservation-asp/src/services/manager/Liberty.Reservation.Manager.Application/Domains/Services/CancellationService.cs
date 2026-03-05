using Liberty.ApplicationShared.Utils;
using Liberty.Cache.Services;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class CancellationService(
    ILogger<CancellationService> logger,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    ICancellationRepository cancellationRepository
) : BaseService<Cancellation>(logger, cacheService, cancellationRepository, new CancellationNotfoundException()),
    ICancellationService
{
    protected override string GetCacheKey(
        string methodName = "",
        params string[] keys
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;
        return $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}:" + base.GetCacheKey(methodName, keys);
    }

    protected override IQueryable<Cancellation> GetQueryable()
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return cancellationRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.FacilityCancellations!.Any(
                    y => y.FacilityId == facilityId
                )
            );
    }

    public override Task<Cancellation> CreateAsync(
        Cancellation entityToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        entityToCreate.Code = EntityUtil.CreateCode();
        entityToCreate.RecordMemo = EntityUtil.CreateRecordMemo();

        return base.CreateAsync(entityToCreate, autoSave, cancellationToken);
    }

    public override Task<Cancellation> UpdateAsync(
        Cancellation entityToUpdate,
        bool autoSave = true,
        Func<Cancellation, Cancellation, Cancellation>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? (
                (
                    existingEntity,
                    updateEntity
                ) =>
                {
                    existingEntity.Name ??= [];
                    existingEntity.Name.UpdateLocalized(updateEntity.Name);
                    existingEntity.Description ??= [];
                    existingEntity.Description.UpdateLocalized(updateEntity.Description);
                    existingEntity.TableSource ??= [];
                    existingEntity.RuleDetail ??= [];
                    existingEntity.RuleDetail.UpdateLocalized(updateEntity.RuleDetail);
                    //existingEntity.TableSource.UpdateLocalized(updateEntity.TableSource);
                    existingEntity.TableSource = updateEntity.TableSource;

                    return existingEntity;
                }
            );

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }
}
