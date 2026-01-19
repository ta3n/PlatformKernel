using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlueprintCqrs.Domain.Entities;

namespace BlueprintCqrs.Domain.Repositories.Interfaces;

public interface IGenericRepository<TEntity, TKey> : IReadOnlyGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
{
    Task<TEntity> CreateOrUpdateAsync(
        TEntity entity
    );

    Task<TEntity> CreateOrUpdateAsync(
        TEntity entity,
        ICollection<Type> entitiesToBeUpdated
    );

    Task DeleteByIdAsync(
        TKey id
    );

    Task DeleteAsync(
        TEntity entity
    );

    Task Clear();

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default
    );

    TEntity Add(
        TEntity entity
    );

    bool AddRange(
        params TEntity[] entities
    );

    TEntity Attach(
        TEntity entity
    );

    TEntity Update(
        TEntity entity
    );

    bool UpdateRange(
        params TEntity[] entities
    );
}
