using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PersonAgeTypeSpaTaxDataService(
    ILogger<PersonAgeTypeSpaTaxDataService> logger,
    IPersonAgeTypeSpaTaxDataRepository repository
)
    : BaseServiceRelation<PersonAgeTypeSpaTaxData>(logger, repository),
        IPersonAgeTypeSpaTaxDataService
{
    public async Task<(
        List<PersonAgeTypeSpaTaxData> addSpaTaxDataOfPersonAgeTypes,
        List<PersonAgeTypeSpaTaxData> updateSpaTaxDataOfPersonAgeTypes,
        List<PersonAgeTypeSpaTaxData> removeSpaTaxDataOfPersonAgeTypes
        )> ChangeSpaTaxDataOfPersonAgeType(
        long id,
        long facilityId,
        List<PersonAgeTypeSpaTaxData> listPersonAgeTypeSpaTaxData,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var listPersonAgeTypeSpaTaxDataExits = await FindAllByFacilityIdAsync(
            id,
            facilityId,
            cancellationToken
        );

        var comparer = new DelegateEqualityComparer<PersonAgeTypeSpaTaxData>(
            (
                    x,
                    y
                ) =>
                x?.PersonAgeTypeId == y?.PersonAgeTypeId
                && x!.SpaTaxData!.PriceMin == y?.SpaTaxData!.PriceMin
                && x.SpaTaxData.PriceMax == y?.SpaTaxData!.PriceMax,
            obj =>
                obj.PersonAgeTypeId.GetHashCode()
                ^ obj.SpaTaxData!.PriceMin.GetHashCode()
                ^ obj.SpaTaxData!.PriceMax.GetHashCode()
        );

        var addSpaTaxDataOfPersonAgeTypes = listPersonAgeTypeSpaTaxData
            .Except(listPersonAgeTypeSpaTaxDataExits, comparer)
            .ToList();

        var removeSpaTaxDataOfPersonAgeTypes = listPersonAgeTypeSpaTaxDataExits
            .Except(listPersonAgeTypeSpaTaxData, comparer)
            .ToList();

        var updateSpaTaxDataOfPersonAgeTypes = listPersonAgeTypeSpaTaxDataExits
            .Intersect(listPersonAgeTypeSpaTaxData, comparer)
            .ToList();

        foreach (var spaTaxData in updateSpaTaxDataOfPersonAgeTypes)
        {
            var matchingData = listPersonAgeTypeSpaTaxData.Find(
                p =>
                    comparer.Equals(p, spaTaxData)
            );
            if (matchingData is not null)
            {
                spaTaxData.SpaTaxData!.Tax = matchingData.SpaTaxData!.Tax;
            }
        }

        if (addSpaTaxDataOfPersonAgeTypes.Count > 0)
        {
            _ = await CreateRangeAsync(addSpaTaxDataOfPersonAgeTypes, autoSave, cancellationToken);
        }

        if (updateSpaTaxDataOfPersonAgeTypes.Count > 0)
        {
            _ = await UpdateRangeAsync(updateSpaTaxDataOfPersonAgeTypes, autoSave, cancellationToken);
        }

        if (removeSpaTaxDataOfPersonAgeTypes.Count > 0)
        {
            _ = await DeleteRangeAsync(removeSpaTaxDataOfPersonAgeTypes, autoSave, cancellationToken);
        }

        return (
            addSpaTaxDataOfPersonAgeTypes,
            updateSpaTaxDataOfPersonAgeTypes,
            removeSpaTaxDataOfPersonAgeTypes
        );
    }

    private async Task<List<PersonAgeTypeSpaTaxData>> FindAllByFacilityIdAsync(
        long id,
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        return await repository
            .GetQueryable()
            .Include(x => x.SpaTaxData)
            .Include(x => x.PersonAgeType)
            .ThenInclude(x => x!.FacilityPersonAgeTypes)
            .Where(x => x.PersonAgeTypeId == id)
            .Where(
                x =>
                    x.PersonAgeType!.FacilityPersonAgeTypes!.Any(y => y.FacilityId == facilityId)
            )
            .Select(
                x => new PersonAgeTypeSpaTaxData
                {
                    PersonAgeTypeId = x.PersonAgeTypeId,
                    SpaTaxDataId = x.SpaTaxDataId,
                    SpaTaxData = new SpaTaxData
                    {
                        PriceMin = x.SpaTaxData!.PriceMin,
                        Id = x.SpaTaxData!.Id,
                        PriceMax = x.SpaTaxData!.PriceMax,
                        Tax = x.SpaTaxData!.Tax
                    }
                }
            )
            .ToListAsync(cancellationToken);
    }
}
