using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IPlanService : IBaseService<Plan>
{
    Task<Plan?> FindByRoomOnlyType(
        CancellationToken cancellationToken = default
    );

    Task<Plan> FindByIdWithIncludeAsync(
        long id,
        CancellationToken cancellationToken = default
    );

    Task<Plan> CreatePlanWithRoomOnlyTypeAsync(
        string? roomGroupName,
        long facilityId,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<PlanRoomGroup> CreatePlanWithRoomOnlyTypeRelationAsync(
        long roomGroupId,
        Plan plan,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<Plan?> GetPlanWithRoomOnlyTypeAsync(
        long roomGroupId,
        long facilityId,
        CancellationToken cancellationToken = default
    );

    Task<Plan> UpdateOverviewAsync(
        Plan entityToUpdate,
        bool autoSave = true,
        Func<Plan, Plan, Plan>? updateAction = null,
        CancellationToken cancellationToken = default
    );

    Task<Plan> UpdateImportantNoteAsync(
        Plan entityToUpdate,
        bool autoSave = true,
        Func<Plan, Plan, Plan>? updateAction = null,
        CancellationToken cancellationToken = default
    );

    Task<Plan> UpdateBasicSettingAsync(
        Plan entityToUpdate,
        bool autoSave = true,
        Func<Plan, Plan, Plan>? updateAction = null,
        CancellationToken cancellationToken = default
    );

    Task<(IEnumerable<FilePlan> adds, IEnumerable<FilePlan> removes)> ChangeFilePlanAsync(
        Plan plan,
        List<FilePlan> filePlans,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task UpdateLastModifiedAsync(
        long id,
        CancellationToken cancellationToken = default
    );

    Task<bool> AnyAsync(
        CancellationToken cancellationToken = default
    );

    Task<bool> IsAnyPlanEnabledUsingCancellationAsync(
        List<long> cancellationIds,
        CancellationToken cancellationToken = default
    );
}
