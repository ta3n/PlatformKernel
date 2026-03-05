using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class SystemConfigService(
    ILogger<SystemConfigService> logger,
    ISystemConfigRepository systemConfigRepository
) : BaseService<SystemConfig>(logger, systemConfigRepository, new SystemConfigNotfoundException()),
    ISystemConfigService
{
    public override Task<SystemConfig> UpdateAsync(
        SystemConfig entityToUpdate,
        bool autoSave = true,
        Func<SystemConfig, SystemConfig, SystemConfig>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.TemplateFormatData = updateEntity.TemplateFormatData;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public async Task UpdateCanOnlinePaymentOfSystemConfigAsync(
        SystemConfig entityToUpdate,
        CancellationToken cancellationToken = default
    )
    {
        await systemConfigRepository
            .GetQueryable()
            .Where(x => x.Id == entityToUpdate.Id)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(b => b.CanOnlinePayment, entityToUpdate.CanOnlinePayment),
                cancellationToken
            );
    }
}
