using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class FacilityAllergenService(
    ILogger<FacilityAllergenService> logger,
    IFacilityAllergenRepository repository
) : BaseServiceRelation<FacilityAllergen>(logger, repository), IFacilityAllergenService
{
    public async Task<(IEnumerable<FacilityAllergen> adds, IEnumerable<FacilityAllergen> removes)>
        ChangeFacilityAllergenAsync(
            long facilityId,
            IEnumerable<long> facilityAllergenIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var existingFacilityAllergen = (await GetAllFacilityAllergenByFacilityIdAsync(
            facilityId,
            cancellationToken
        )).ToList();
        var updateFacilityAllergen = facilityAllergenIds
            .Select(
                allergenId => new FacilityAllergen
                {
                    FacilityId = facilityId,
                    AllergenId = allergenId
                }
            )
            .ToList();

        var comparer = new DelegateEqualityComparer<FacilityAllergen>(
            (
                x,
                y
            ) => x?.FacilityId == y?.FacilityId && x!.AllergenId == y?.AllergenId,
            obj => obj.FacilityId.GetHashCode() ^ obj.AllergenId.GetHashCode()
        );

        var addFacilityAllergen = updateFacilityAllergen.Except(
            existingFacilityAllergen,
            comparer
        );

        var adds = await CreateRangeAsync(
            addFacilityAllergen,
            autoSave,
            cancellationToken
        );

        IEnumerable<FacilityAllergen> removes = [];
        if (existingFacilityAllergen.Count > 0)
        {
            var removeFacilityAllergen = existingFacilityAllergen.Except(
                updateFacilityAllergen,
                comparer
            );
            removes = await DeleteRangeAsync(
                removeFacilityAllergen,
                autoSave,
                cancellationToken
            );
        }

        return (adds, removes);
    }

    private async Task<List<FacilityAllergen>> GetAllFacilityAllergenByFacilityIdAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        return await repository.GetQueryableWithAsNoTracking()
            .Where(x => x.FacilityId == facilityId)
            .ToListAsync(cancellationToken);
    }
}
