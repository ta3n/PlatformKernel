using Liberty.Cache.Services;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class FacilityService(
    ILogger<FacilityService> logger,
    ICacheService cacheService,
    IFacilityRepository facilityRepository
) : BaseService<Facility>(logger, cacheService, facilityRepository, new FacilityNotfoundException()), IFacilityService
{
    public override Task<Facility> UpdateAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.CanOnLinePayment = updateEntity.CanOnLinePayment;
                existingEntity.Meta!.SystemEMail = updateEntity.Meta!.SystemEMail;
                existingEntity.Memo = updateEntity.Memo;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public Task<Facility> UpdateFaxOfFacilityAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            (
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.Fax = updateEntity.Fax;
                existingEntity.UseFax = updateEntity.UseFax;

                return existingEntity;
            },
            cancellationToken
        );
    }

    public async Task<IEnumerable<Facility>> FindAllFacilitiesHasCategoryAsync(
        long categoryId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.CategoryId == categoryId);

        var facilities = await queryable.ToListAsync(cancellationToken);

        return facilities;
    }
}
