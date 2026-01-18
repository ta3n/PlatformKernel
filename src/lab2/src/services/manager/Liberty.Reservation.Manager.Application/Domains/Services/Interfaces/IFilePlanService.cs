using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IFilePlanService : IBaseServiceRelation<FilePlan>
{
    Task<List<FilePlan>> GetAllFilePlanByPlanIdAsync(
        long planId,
        CancellationToken cancellationToken = default
    );

    Task<(IEnumerable<FilePlan> adds, IEnumerable<FilePlan> removes)> ChangeFilePlanAsync(
        long planId,
        List<FilePlan> filePlans,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<List<FilePlan>> GetByFileIdAsync(
        long fileId,
        CancellationToken cancellationToken = default
    );
}
