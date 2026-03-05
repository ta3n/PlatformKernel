using Liberty.Cache.Services;
using Liberty.Reservation.Application.Exceptions;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PlanDistributionService(
    ILogger<PlanDistributionService> logger,
    IPlanRepository planRepository,
    IFacilityService facilityService,
    ICacheService cacheService
) : BaseService<Plan>(logger, planRepository, new PlanNotfoundException()),
    IPlanDistributionService
{
    public async Task<Plan> UpdatePlanAsync(
        Plan entityToUpdate,
        string facilityCode,
        bool autoSave = true,
        Func<Plan, Plan, Plan>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? (
                (
                    _,
                    updateEntity
                ) => updateEntity);

        var facilityId = await facilityService.GetFacilityIdByCodeAsync(facilityCode, cancellationToken);
        await cacheService.ResetAsync(
            string.Format(CacheKeys.ResetPatternSiteBookingSearch, facilityId),
            false,
            cancellationToken
        );

        return await base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public async Task<long> GetPlanIdByPlanCode(
        string? planCode,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = GetQueryable()
            .Where(x => x.Code == planCode)
            .Select(x => x.Id);
        var planId = await queryable.SingleOrDefaultAsync(cancellationToken);

        return planId;
    }

    public async Task<Plan?> FindPlanByCode(
        string planCode,
        CancellationToken cancellationToken = default
    )
    {
        var cacheKey = GetCacheKey(
            nameof(FindPlanByCode),
            $"{planCode}"
        );
        Plan? existingEntity = null;
        if (CacheService is not null)
        {
            existingEntity = await CacheService.GetAsync<Plan>(
                cacheKey,
                cancellationToken
            );
        }

        if (existingEntity is null)
        {
            var queryable = GetQueryable();
            existingEntity = await queryable.SingleOrDefaultAsync(
                x => x.Code == planCode,
                cancellationToken
            );

            if (CacheService is not null)
            {
                await CacheService.SetAsync(
                    cacheKey,
                    existingEntity,
                    cancellationToken
                );
            }
        }

        return existingEntity;
    }
}
