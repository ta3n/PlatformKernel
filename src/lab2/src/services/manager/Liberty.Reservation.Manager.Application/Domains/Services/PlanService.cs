using Liberty.ApplicationShared.Utils;
using Liberty.Cache.Services;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PlanService(
    ILogger<PlanService> logger,
    IPlanRepository planRepository,
    IFacilityPlanRepository facilityPlanRepository,
    ISecurityContextAccessor securityContextAccessor,
    ICacheService cacheService,
    IPlanRoomGroupService planRoomGroupService,
    IFilePlanService filePlanService
) : BaseService<Plan>(logger, planRepository, new PlanNotfoundException()), IPlanService
{
    protected override IQueryable<Plan> GetQueryable()
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return base.GetQueryable()
            .Where(
                x => x.FacilityPlans!.Any(
                    t => t.FacilityId == facilityId
                )
            );
    }

    public async Task<Plan> FindByIdWithIncludeAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        return await GetQueryable()
                .Include(x => x.PointRate)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken
                )
            ?? throw new PlanNotfoundException();
    }

    public async Task<Plan?> FindByRoomOnlyType(
        CancellationToken cancellationToken = default
    )
    {
        return await planRepository.GetQueryableWithAsNoTracking()
            .Include(x => x.FacilityPlans)
            .Where(
                x => x.PlanType == PlanTypes.RoomOnly
                    && x.FacilityPlans!.Any(
                        t => t.FacilityId == securityContextAccessor.FacilityKey
                    )
            )
            .FirstOrDefaultAsync(cancellationToken);
    }

    public override async Task<Plan> UpdateAsync(
        Plan entityToUpdate,
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

        var facilityId = securityContextAccessor.FacilityKey;

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

    public async Task<Plan> CreatePlanWithRoomOnlyTypeAsync(
        string? roomGroupName,
        long facilityId,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var planForRoomOnly = new Plan
        {
            Code = EntityUtil.CreateCode(),
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), $"Plan for room: {roomGroupName}" } },
            PlanType = PlanTypes.RoomOnly,
            IsEnabled = true,
            IsOnSidePayment = true,
            IsOnLinePayment = true
        };

        var newPlanForRoomOnly = await planRepository.AddAsync(
            planForRoomOnly,
            autoSave,
            cancellationToken
        );

        var facilityPlan = new FacilityPlan
        {
            FacilityId = facilityId,
            Plan = newPlanForRoomOnly,
            IsEnabled = true
        };

        _ = await facilityPlanRepository.AddAsync(
            facilityPlan,
            autoSave,
            cancellationToken
        );

        return planForRoomOnly;
    }

    public async Task<PlanRoomGroup> CreatePlanWithRoomOnlyTypeRelationAsync(
        long roomGroupId,
        Plan plan,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var planRoomGroup = new PlanRoomGroup
        {
            RoomGroupId = roomGroupId,
            Plan = plan,
            IsEnabled = true
        };

        _ = await planRoomGroupService.CreateAsync(planRoomGroup, autoSave, cancellationToken);

        return planRoomGroup;
    }

    public async Task<Plan?> GetPlanWithRoomOnlyTypeAsync(
        long roomGroupId,
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        var plan = await planRoomGroupService.GetPlanWithRoomOnlyTypeAsync(roomGroupId, facilityId, cancellationToken);

        return plan;
    }

    public Task<Plan> UpdateOverviewAsync(
        Plan entityToUpdate,
        bool autoSave = true,
        Func<Plan, Plan, Plan>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.Summary ??= [];
                existingEntity.Summary.UpdateLocalized(updateEntity.Summary);

                existingEntity.Description ??= [];
                existingEntity.Description.UpdateLocalized(updateEntity.Description);

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public Task<Plan> UpdateBasicSettingAsync(
        Plan entityToUpdate,
        bool autoSave = true,
        Func<Plan, Plan, Plan>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.Name ??= [];
                existingEntity.Name.UpdateLocalized(updateEntity.Name);
                existingEntity.NameForImport ??= [];
                existingEntity.NameForImport.UpdateLocalized(updateEntity.NameForImport);
                existingEntity.Description ??= [];
                existingEntity.Description.UpdateLocalized(updateEntity.Description);
                existingEntity.Summary ??= [];
                existingEntity.Summary.UpdateLocalized(updateEntity.Summary);

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public Task<Plan> UpdateImportantNoteAsync(
        Plan entityToUpdate,
        bool autoSave = true,
        Func<Plan, Plan, Plan>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.Meal ??= [];
                existingEntity.Meal.UpdateLocalized(updateEntity.Meal);
                existingEntity.Other ??= [];
                existingEntity.Other.UpdateLocalized(updateEntity.Other);
                existingEntity.Payment ??= [];
                existingEntity.Payment.UpdateLocalized(updateEntity.Payment);

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public async Task<(IEnumerable<FilePlan> adds, IEnumerable<FilePlan> removes)>
        ChangeFilePlanAsync(
            Plan plan,
            List<FilePlan> filePlans,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var existingFilePlan = (await filePlanService.GetAllFilePlanByPlanIdAsync(
            plan.Id,
            cancellationToken
        )).ToList();
        var updateFilePlan = filePlans
            .Select(
                filePlan => new FilePlan
                {
                    PlanId = plan.Id,
                    Plan = plan.Id == 0 ? plan : null,
                    FileId = filePlan.FileId,
                    Index = filePlan.Index
                }
            )
            .ToList();

        var comparer = new DelegateEqualityComparer<FilePlan>(
            (
                x,
                y
            ) => x?.FileId == y?.FileId && x?.PlanId == y?.PlanId,
            obj => obj.FileId.GetHashCode() ^ obj.PlanId.GetHashCode()
        );

        var removeFilePlan = existingFilePlan.Except(
            updateFilePlan,
            comparer
        );
        var addFilePlan = updateFilePlan.Except(
            existingFilePlan,
            comparer
        );

        var removes = await filePlanService.DeleteRangeAsync(
            removeFilePlan,
            autoSave,
            cancellationToken
        );

        var adds = await filePlanService.CreateRangeAsync(
            addFilePlan,
            autoSave,
            cancellationToken
        );
        return (adds, removes);
    }

    public async Task UpdateLastModifiedAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        var longDate = ConvertUtil.ToLong(
            ConvertUtil.ToString(DateTime.UtcNow, "yyyyMMddHHmmssfff")
        );

        await planRepository
            .GetQueryable()
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(b => b.UpdatedAt, longDate),
                cancellationToken
            );
    }

    public async Task<bool> AnyAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await GetQueryable().AnyAsync(cancellationToken);
    }

    public async Task<bool> IsAnyPlanEnabledUsingCancellationAsync(
        List<long> cancellationIds,
        CancellationToken cancellationToken = default
    )
    {
        return await planRepository.GetQueryableWithAsNoTracking()
            .AnyAsync(
                x => x.CancellationId != null && x.IsEnabled && cancellationIds.Contains(x.CancellationId ?? 0),
                cancellationToken
            );
    }
}
