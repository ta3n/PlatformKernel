using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class AppDateDataService(
    ILogger<AppDateDataService> logger,
    IAppDateDataRepository appDateDataRepository
) : BaseService<AppDateData>(logger, appDateDataRepository, new AppDateDataNotfoundException()), IAppDateDataService
{
    public override Task<AppDateData> CreateAsync(
        AppDateData entityToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        entityToCreate.Code = EntityUtil.CreateCode();
        entityToCreate.RecordMemo = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        return base.CreateAsync(entityToCreate, autoSave, cancellationToken);
    }

    public async Task<AppDateData> DeleteByDateIdAsync(
        long appDateId,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var existingEntity = await FindByDateIdAsync(
                appDateId,
                cancellationToken
            )
            ?? throw new AppDateDataNotfoundException();

        return await DeleteAsync(
            existingEntity.Id,
            autoSave,
            cancellationToken
        );
    }

    public async Task<AppDateData?> FindByDateIdAsync(
        long appDateId,
        CancellationToken cancellationToken = default
    )
    {
        var existingEntity = await appDateDataRepository
            .GetQueryableWithAsNoTracking()
            .SingleOrDefaultAsync(
                x => x.DisplayOrder == appDateId,
                cancellationToken
            );

        return existingEntity;
    }
}
