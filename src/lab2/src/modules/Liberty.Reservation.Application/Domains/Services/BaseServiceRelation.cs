using Liberty.Entity;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Application.Domains.Services;

public class BaseServiceRelation<TEntity>(
    ILogger logger,
    IRepositoryBase<TEntity> entityRepository
) : IBaseServiceRelation<TEntity> where TEntity : EntityRelation
{
    public async Task<TEntity> CreateAsync(
        TEntity entityToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var addEntity = await entityRepository.AddAsync(
            entityToCreate,
            autoSave,
            cancellationToken
        );

        logger.LogInformation(
            "{BaseServiceRelationName}.{RangeAsyncName} - Successfully created new {EntityRelationName}",
            nameof(BaseServiceRelation<TEntity>),
            nameof(CreateRangeAsync),
            nameof(TEntity)
        );

        return addEntity;
    }

    public async Task<IEnumerable<TEntity>> CreateRangeAsync(
        IEnumerable<TEntity> entitiesToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var addEntities = (await entityRepository.AddRangeAsync(
            entitiesToCreate,
            autoSave,
            cancellationToken
        )).ToList();

        logger.LogInformation(
            "{BaseServiceRelationName}.{RangeAsyncName} - Successfully created new {EntityRelationName} {Count}",
            nameof(BaseServiceRelation<TEntity>),
            nameof(CreateRangeAsync),
            nameof(TEntity),
            addEntities.Count
        );

        return addEntities;
    }

    public async Task<TEntity> UpdateAsync(
        TEntity entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var entityEdit = await entityRepository.UpdateAsync(
            entityToUpdate,
            autoSave,
            cancellationToken
        );

        logger.LogInformation(
            "{BaseServiceRelationName}.{UpdateAsyncName} - Successfully updated {EntityDataName}",
            nameof(BaseServiceRelation<TEntity>),
            nameof(UpdateAsync),
            nameof(TEntity)
        );

        return entityEdit;
    }

    public async Task<IEnumerable<TEntity>> UpdateRangeAsync(
        IEnumerable<TEntity> entitiesToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var editEntities = (await entityRepository.UpdateRangeAsync(
            entitiesToUpdate,
            autoSave,
            cancellationToken
        )).ToList();

        logger.LogInformation(
            "{BaseServiceRelationName}.{UpdateAsyncName} - Successfully updated {EntityDataName} {Count}",
            nameof(BaseServiceRelation<TEntity>),
            nameof(UpdateRangeAsync),
            nameof(TEntity),
            editEntities.Count
        );

        return editEntities;
    }

    public async Task<TEntity> DeleteAsync(
        TEntity entityToDelete,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        entityToDelete.IsDeleted = true;
        await entityRepository.DeleteAsync(
            entityToDelete,
            autoSave,
            cancellationToken
        );

        logger.LogInformation(
            "{BaseServiceRelationName}.{RangeAsyncName} - Successfully deleted {EntityRelationName}",
            nameof(BaseServiceRelation<TEntity>),
            nameof(CreateRangeAsync),
            nameof(TEntity)
        );

        return entityToDelete;
    }

    public async Task<IEnumerable<TEntity>> DeleteRangeAsync(
        IEnumerable<TEntity> entitiesToDelete,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var entityRelations = entitiesToDelete as TEntity[] ?? [.. entitiesToDelete];

        await entityRepository.DeleteRangeAsync(
            entityRelations,
            autoSave,
            cancellationToken
        );

        logger.LogInformation(
            "{BaseServiceRelationName}.{DeleteRangeAsyncName} - Successfully deleted {EntityRelationName} {Count}",
            nameof(BaseServiceRelation<TEntity>),
            nameof(DeleteRangeAsync),
            nameof(TEntity),
            entityRelations.Length
        );

        return entityRelations;
    }
}
