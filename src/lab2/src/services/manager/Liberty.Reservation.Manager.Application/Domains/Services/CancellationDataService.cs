namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class CancellationDataService(
    ILogger<CancellationDataService> logger,
    ICancellationDataRepository cancellationDataRepository
) : BaseService<CancellationData>(logger, cancellationDataRepository, new CancellationDataNotFoundException()),
    ICancellationDataService
{
    public override Task<CancellationData> CreateAsync(
        CancellationData entityToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        entityToCreate.RecordMemo = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        return base.CreateAsync(entityToCreate, autoSave, cancellationToken);
    }

    public override Task<IEnumerable<CancellationData>> CreateRangeAsync(
        IEnumerable<CancellationData> entitiesToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var entities = entitiesToCreate.ToList();
        foreach (var entityToCreate in entities)
        {
            entityToCreate.RecordMemo = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        }

        return base.CreateRangeAsync(
            entities,
            autoSave,
            cancellationToken
        );
    }

    public override Task<CancellationData> UpdateAsync(
        CancellationData entityToUpdate,
        bool autoSave = true,
        Func<CancellationData, CancellationData, CancellationData>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.DayStart = updateEntity.DayStart;
                existingEntity.DayEnd = updateEntity.DayEnd;
                existingEntity.Rate = updateEntity.Rate;
                existingEntity.Description ??= [];
                existingEntity.Description?.UpdateLocalized(updateEntity.Description);

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public override Task<IEnumerable<CancellationData>> UpdateRangeAsync(
        IEnumerable<CancellationData> entitiesToUpdate,
        bool autoSave = true,
        Func<CancellationData, CancellationData, CancellationData>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.DayStart = updateEntity.DayStart;
                existingEntity.DayEnd = updateEntity.DayEnd;
                existingEntity.Rate = updateEntity.Rate;
                existingEntity.Description ??= [];
                existingEntity.Description?.UpdateLocalized(updateEntity.Description);
                existingEntity.DisplayOrder = updateEntity.DisplayOrder;

                return existingEntity;
            });

        return base.UpdateRangeAsync(
            entitiesToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public async Task<(IEnumerable<CancellationData> entitiesToCreate, IEnumerable<long> idsToDelete)>
        AdjustRangeAsync(
            IEnumerable<CancellationData> entitiesToEdit,
            IEnumerable<CancellationCancellationData> existingDataOfCancellation,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var entitiesToChange = entitiesToEdit.ToList();
        var entitiesToUpdate = entitiesToChange
            .Where(
                x =>
                    x.Id != default && existingDataOfCancellation.Any(c => c.CancellationDataId == x.Id)
            )
            .ToList();
        var entitiesToCreate = entitiesToChange.Except(entitiesToUpdate);
        var idsToDelete = existingDataOfCancellation
            .Where(
                x => !x.IsDeleted
                    && entitiesToChange.TrueForAll(t => t.Id != x.CancellationDataId)
            )
            .Select(x => x.CancellationDataId)
            .ToArray();

        var newEntitiesToCreate = await CreateRangeAsync(
            entitiesToCreate,
            autoSave,
            cancellationToken
        );

        _ = await UpdateRangeAsync(
            entitiesToUpdate,
            autoSave,
            null,
            cancellationToken
        );

        _ = await DeleteRangeAsync(
            idsToDelete,
            autoSave,
            cancellationToken
        );

        return (newEntitiesToCreate, idsToDelete);
    }
}
