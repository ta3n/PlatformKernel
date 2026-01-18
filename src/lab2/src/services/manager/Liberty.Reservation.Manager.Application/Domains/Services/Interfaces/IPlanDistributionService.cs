using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IPlanDistributionService : IBaseService<Plan>
{
    Task<Plan> UpdatePlanAsync(
        Plan entityToUpdate,
        string facilityCode,
        bool autoSave = true,
        Func<Plan, Plan, Plan>? updateAction = null,
        CancellationToken cancellationToken = default
    );

    Task<long> GetPlanIdByPlanCode(
        string? planCode,
        CancellationToken cancellationToken = default
    );

    Task<Plan?> FindPlanByCode(
        string planCode,
        CancellationToken cancellationToken = default
    );
}
