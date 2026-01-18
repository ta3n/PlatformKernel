using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.Application.Domains.Services.Interfaces;
using Liberty.Reservation.User.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.User.Application.Domains.Services;

public class FacilityService(
    ILogger<FacilityService> logger,
    IFacilityRepository facilityRepository
) : BaseService<Facility>(logger, facilityRepository, new FacilityNotfoundException()), IFacilityService
{
    public override Task<Facility> CreateAsync(
        Facility entityToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        entityToCreate.Code = EntityUtil.CreateCode();
        entityToCreate.RecordMemo = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        return base.CreateAsync(entityToCreate, autoSave, cancellationToken);
    }

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
                existingEntity.Code = updateEntity.Code;
                existingEntity.Memo = updateEntity.Memo;
                existingEntity.CanOnLinePayment = updateEntity.CanOnLinePayment;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public async Task<bool> CheckPaymentOnSitePaymentAvailableAsync(
        long facilityId,
        CancellationToken cancellationToken
    )
    {
        var queryable = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == facilityId)
            .Where(x => x.IsEnabled)
            .Where(x => x.IsOnSidePayment);

        var result = await queryable.AnyAsync(cancellationToken);

        return result;
    }
}
