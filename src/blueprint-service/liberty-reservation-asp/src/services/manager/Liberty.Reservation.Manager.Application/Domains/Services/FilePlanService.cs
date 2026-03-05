using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class FilePlanService(
    ILogger<FilePlanService> logger,
    IFilePlanRepository filePlanRepository
) : BaseServiceRelation<FilePlan>(logger, filePlanRepository), IFilePlanService
{
    public async Task<List<FilePlan>> GetAllFilePlanByPlanIdAsync(
        long planId,
        CancellationToken cancellationToken = default
    )
    {
        return await filePlanRepository.GetQueryableWithAsNoTracking()
            .Where(x => x.PlanId == planId)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<FilePlan> adds, IEnumerable<FilePlan> removes)>
        ChangeFilePlanAsync(
            long planId,
            List<FilePlan> filePlans,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var existingFilePlan = (await GetAllFilePlanByPlanIdAsync(
            planId,
            cancellationToken
        )).ToList();
        var updateFilePlan = filePlans
            .Select(
                filePlan => new FilePlan
                {
                    PlanId = planId,
                    FileId = filePlan.FileId,
                    Index = filePlan.Index
                }
            )
            .ToList();

        var comparer = new DelegateEqualityComparer<FilePlan>(
            (
                x,
                y
            ) => x?.FileId == y?.FileId && x?.Index == y?.Index && x?.PlanId == y?.PlanId,
            obj => obj.FileId.GetHashCode() ^ obj.PlanId.GetHashCode() ^ obj.Index.GetHashCode()
        );

        var removeFilePlan = existingFilePlan.Except(
            updateFilePlan,
            comparer
        );
        var addFilePlan = updateFilePlan.Except(
            existingFilePlan,
            comparer
        );

        var removes = await DeleteRangeAsync(
            removeFilePlan,
            autoSave,
            cancellationToken
        );

        var adds = await CreateRangeAsync(
            addFilePlan,
            autoSave,
            cancellationToken
        );
        return (adds, removes);
    }

    public async Task<List<FilePlan>> GetByFileIdAsync(
        long fileId,
        CancellationToken cancellationToken = default
    )
    {
        var data = await filePlanRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.FileId == fileId
            )
            .ToListAsync(cancellationToken);

        return data;
    }
}
