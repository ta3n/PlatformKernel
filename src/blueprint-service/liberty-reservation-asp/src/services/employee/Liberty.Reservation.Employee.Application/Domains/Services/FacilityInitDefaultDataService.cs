using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class FacilityInitDefaultDataService(
    ILogger<FacilityInitDefaultDataService> logger,
    IFacilityRepository facilityRepository,
    IFacilityPersonAgeTypeRepository facilityPersonAgeTypeRepository,
    IPersonAgeTypeRepository personAgeTypeRepository,
    ISpaTaxDataRepository spaTaxDataRepository,
    IPersonAgeTypeSpaTaxDataRepository personAgeTypeSpaTaxDataRepository
) : BaseService<Facility>(logger, facilityRepository, new FacilityNotfoundException()), IFacilityInitDefaultDataService
{
    public async Task<IEnumerable<FacilityPersonAgeType>> AddDefaultPersonAgeTypesAsync(
        Facility facility,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var isExisting = await facilityRepository
            .GetQueryableWithAsNoTracking()
            .AnyAsync(
                x => x.Id == facility.Id && x.FacilityPersonAgeTypes!.Count > 0,
                cancellationToken
            );

        if (isExisting)
        {
            return [];
        }

        var queryableBathingTaxAge = personAgeTypeSpaTaxDataRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.PersonAgeType!.IsMaster)
            .OrderByDescending(x => x.PersonAgeType!.Id)
            .Select(
                x => new PersonAgeTypeSpaTaxData
                {
                    SpaTaxData = new SpaTaxData
                    {
                        PriceMin = x.SpaTaxData!.PriceMin,
                        PriceMax = x.SpaTaxData!.PriceMax,
                        Tax = x.SpaTaxData!.Tax,
                        IsEnabled = true
                    },
                    PersonAgeType = new PersonAgeType
                    {
                        Name = x.PersonAgeType!.Name,
                        IsMain = x.PersonAgeType!.IsMain,
                        AgeMax = x.PersonAgeType!.AgeMax,
                        AgeMin = x.PersonAgeType!.AgeMin,
                        Meta = x.PersonAgeType!.Meta,
                        DisplayOrder = x.PersonAgeType.DisplayOrder,
                        IsMaster = false,
                        IsEnabled = true
                    }
                }
            );

        var addListPersonAgeTypeSpaTaxDataOfFacility = await queryableBathingTaxAge.ToListAsync(
            cancellationToken
        );

        foreach (var data in addListPersonAgeTypeSpaTaxDataOfFacility)
        {
            data.PersonAgeType!.Code = EntityUtil.CreateCode();
            data.SpaTaxData!.Code = EntityUtil.CreateCode();
        }

        var addNewPersonAgeTypes = (await personAgeTypeRepository.AddRangeAsync(
            addListPersonAgeTypeSpaTaxDataOfFacility.Select(x => x.PersonAgeType!),
            false,
            cancellationToken
        )).ToList();

        var addNewSpaTaxData = (await spaTaxDataRepository.AddRangeAsync(
            addListPersonAgeTypeSpaTaxDataOfFacility.Select(x => x.SpaTaxData!),
            false,
            cancellationToken
        )).ToList();

        var personAgeTypesOfFacility = new List<FacilityPersonAgeType>();

        for (var i = 0; i < addListPersonAgeTypeSpaTaxDataOfFacility.Count; i++)
        {
            var personAgeType = addNewPersonAgeTypes[i];
            var spaTaxData = addNewSpaTaxData[i];

            await personAgeTypeSpaTaxDataRepository.AddAsync(
                new PersonAgeTypeSpaTaxData
                {
                    PersonAgeType = personAgeType,
                    SpaTaxData = spaTaxData
                },
                autoSave,
                cancellationToken
            );

            var addPersonAgeTypeOfFacility = facility.Id > 0
                ? new FacilityPersonAgeType
                {
                    FacilityId = facility.Id,
                    PersonAgeType = personAgeType
                }
                : new FacilityPersonAgeType
                {
                    Facility = facility,
                    PersonAgeType = personAgeType
                };

            personAgeTypesOfFacility.Add(addPersonAgeTypeOfFacility);
        }

        var response = await facilityPersonAgeTypeRepository.AddRangeAsync(
            personAgeTypesOfFacility,
            autoSave,
            cancellationToken
        );

        return response;
    }
}
