using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class PersonAgeTypeSpaTaxDataService(
    ILogger<PersonAgeTypeSpaTaxData> logger,
    IPersonAgeTypeSpaTaxDataRepository spaTaxDataRepository
) : BaseServiceRelation<PersonAgeTypeSpaTaxData>(logger, spaTaxDataRepository), IPersonAgeTypeSpaTaxDataService
{
    public async Task<(
        List<PersonAgeTypeSpaTaxData> addSpaTaxDataOfPersonAgeTypes,
        List<PersonAgeTypeSpaTaxData> updateSpaTaxDataOfPersonAgeTypes,
        List<PersonAgeTypeSpaTaxData> removeSpaTaxDataOfPersonAgeTypes
        )> ChangeSpaTaxDataOfPersonAgeTypeAsync(
        long id,
        List<PersonAgeTypeSpaTaxData> listPersonAgeTypeSpaTaxData,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var listPersonAgeTypeSpaTaxDataExits = await FindAllByPersonAgeTypeIdAsync(
            id,
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

    private async Task<List<PersonAgeTypeSpaTaxData>> FindAllByPersonAgeTypeIdAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        return await spaTaxDataRepository
            .GetQueryable()
            .Where(x => x.PersonAgeTypeId == id)
            .Select(
                x => new PersonAgeTypeSpaTaxData
                {
                    PersonAgeTypeId = x.PersonAgeTypeId,
                    SpaTaxDataId = x.SpaTaxDataId,
                    SpaTaxData = x.SpaTaxData != null
                        ? new SpaTaxData
                        {
                            PriceMax = x.SpaTaxData.PriceMax,
                            PriceMin = x.SpaTaxData.PriceMin,
                            Id = x.SpaTaxData.Id,
                            Tax = x.SpaTaxData.Tax
                        }
                        : new SpaTaxData()
                }
            )
            .ToListAsync(cancellationToken);
    }
}
