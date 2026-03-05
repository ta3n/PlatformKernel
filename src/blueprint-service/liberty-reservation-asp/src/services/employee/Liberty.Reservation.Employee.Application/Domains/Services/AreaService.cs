using Liberty.ApplicationShared.Utils;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class AreaService(
    ILogger<AreaService> logger,
    ICacheService cacheService,
    IAreaRepository areaRepository
) : BaseService<Area>(logger, cacheService, areaRepository, new AreaNotfoundException()), IAreaService
{
    public override Task<Area> CreateAsync(
        Area entityToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        entityToCreate.Code = EntityUtil.CreateCode();
        entityToCreate.RecordMemo = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        return base.CreateAsync(entityToCreate, autoSave, cancellationToken);
    }

    public override Task<Area> UpdateAsync(
        Area entityToUpdate,
        bool autoSave = true,
        Func<Area, Area, Area>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.Name = updateEntity.Name;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }
}
