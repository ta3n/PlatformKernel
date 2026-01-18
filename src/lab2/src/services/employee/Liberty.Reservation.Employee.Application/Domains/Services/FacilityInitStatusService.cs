using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Liberty.Reservation.Employee.Application.Models;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class FacilityInitStatusService(
    ILogger<FacilityInitDefaultDataService> logger,
    IFacilityRepository facilityRepository
) : BaseService<Facility>(logger, facilityRepository, new FacilityNotfoundException()), IFacilityInitStatusService
{
    public async Task<FacilitySeedDataStatus> GetFacilitySeedDataStatusAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        var existingFacilityQueryable = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == facilityId)
            .Select(
                x => new
                {
                    x.Id,
                    x.Code,
                    x.Name
                }
            );
        var existingFacility = await existingFacilityQueryable.FirstOrDefaultAsync(
            cancellationToken
        );

        var hasSeededPersonAgeTypeData = await CheckHasSeededPersonAgeTypeDataAsync(
            facilityId,
            cancellationToken
        );

        return new FacilitySeedDataStatus(
            existingFacility?.Id,
            existingFacility?.Code,
            existingFacility?.Name,
            hasSeededPersonAgeTypeData
        );
    }

    private async Task<bool> CheckHasSeededPersonAgeTypeDataAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        var isExisting = await facilityRepository
            .GetQueryableWithAsNoTracking()
            .AnyAsync(
                x => x.Id == facilityId && x.FacilityPersonAgeTypes!.Count > 0,
                cancellationToken
            );

        return isExisting;
    }
}
