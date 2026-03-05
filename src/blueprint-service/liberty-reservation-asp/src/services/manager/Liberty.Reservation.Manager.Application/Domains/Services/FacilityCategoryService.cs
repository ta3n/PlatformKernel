using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class FacilityCategoryService(
    ILogger<FacilityCategoryService> logger,
    IFacilityCategoryRepository repository
) : BaseServiceRelation<FacilityCategory>(logger, repository), IFacilityCategoryService
{
    public async Task<(IEnumerable<FacilityCategory> adds, IEnumerable<FacilityCategory> removes)>
        ChangeFacilityCategoryAsync(
            long facilityId,
            List<long> facilityCategoryIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var existingFacilityCategory = (await GetAllFacilityCategoryByFacilityIdAsync(
            facilityId,
            cancellationToken
        )).ToList();
        var updateFacilityCategory = facilityCategoryIds
            .Select(
                categoryId => new FacilityCategory
                {
                    FacilityId = facilityId,
                    CategoryId = categoryId
                }
            )
            .ToList();

        var comparer = new DelegateEqualityComparer<FacilityCategory>(
            (
                x,
                y
            ) => x?.FacilityId == y?.FacilityId && x!.CategoryId == y?.CategoryId,
            obj => obj.FacilityId.GetHashCode() ^ obj.CategoryId.GetHashCode()
        );

        var addFacilityCategory = updateFacilityCategory.Except(
            existingFacilityCategory,
            comparer
        );

        var adds = await CreateRangeAsync(
            addFacilityCategory,
            autoSave,
            cancellationToken
        );

        IEnumerable<FacilityCategory> removes = [];
        if (existingFacilityCategory.Count > 0)
        {
            var removeFacilityCategory = existingFacilityCategory.Except(
                updateFacilityCategory,
                comparer
            );
            removes = await DeleteRangeAsync(
                removeFacilityCategory,
                autoSave,
                cancellationToken
            );
        }

        return (adds, removes);
    }

    private async Task<List<FacilityCategory>> GetAllFacilityCategoryByFacilityIdAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        return await repository.GetQueryableWithAsNoTracking()
            .Where(x => x.FacilityId == facilityId)
            .ToListAsync(cancellationToken);
    }
}
