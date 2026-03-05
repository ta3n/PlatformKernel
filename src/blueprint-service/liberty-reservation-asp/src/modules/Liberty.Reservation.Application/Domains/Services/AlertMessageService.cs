using Liberty.Cache.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Application.Domains.Services;

public class AlertMessageService(
    ILogger<AlertMessageService> logger,
    ICacheService cacheService,
    IAlertMessageRepository alertMessageRepository
) : BaseService<AlertMessage>(logger, cacheService, alertMessageRepository, new AlertMessageNotFoundException()), IAlertMessageService
{
    public override Task<AlertMessage> UpdateAsync(
        AlertMessage entityToUpdate,
        bool autoSave = true,
        Func<AlertMessage, AlertMessage, AlertMessage>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.Title ??= [];
                existingEntity.Title.UpdateLocalized(updateEntity.Title);
                existingEntity.Content ??= [];
                existingEntity.Content.UpdateLocalized(updateEntity.Content);
                existingEntity.IsEnabled = updateEntity.IsEnabled;
                existingEntity.Color = updateEntity.Color;
                existingEntity.Icon = updateEntity.Icon;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public async Task<AlertMessage> GetFirstAlertMessageAsync(
        CancellationToken cancellationToken = default
    )
    {
        var alertMessage = await alertMessageRepository
                .GetQueryableWithAsNoTracking()
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new AlertMessageNotFoundException();

        return alertMessage;
    }

    public async Task<bool> ExistingAlertMessageAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        return await alertMessageRepository
            .GetQueryableWithAsNoTracking()
            .AnyAsync(x => x.Id == id, cancellationToken);
    }
}
