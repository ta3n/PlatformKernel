using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlueprintCqrs.Domain.Entities;
using BlueprintCqrs.Domain.Repositories.Interfaces;

namespace BlueprintCqrs.Infrastructure.Data.Repositories;

public abstract class GenericRepository<TEntity, TKey>(
    IUnitOfWork context
) : ReadOnlyGenericRepository<TEntity, TKey>(context), IGenericRepository<TEntity, TKey>, IDisposable where TEntity : BaseEntity<TKey>
{
    public virtual TEntity Add(
        TEntity entity
    )
    {
        DbSet.Add(entity);
        return entity;
    }

    public virtual bool AddRange(
        params TEntity[] entities
    )
    {
        DbSet.AddRange(entities);
        return true;
    }

    public virtual TEntity Attach(
        TEntity entity
    )
    {
        var entry = DbSet.Attach(entity);
        entry.State = EntityState.Added;
        return entity;
    }

    public virtual TEntity Update(
        TEntity entity
    )
    {
        DbSet.Update(entity);
        return entity;
    }

    public virtual TEntity Update(
        string id,
        TEntity entity
    )
    {
        DbSet.Update(entity);
        return entity;
    }

    public virtual bool UpdateRange(
        params TEntity[] entities
    )
    {
        DbSet.UpdateRange(entities);
        return true;
    }

    public virtual async Task<TEntity> CreateOrUpdateAsync(
        TEntity entity
    )
    {
        var exists = await Exists(x => x.Id.Equals(entity.Id));
        if (entity.Id.Equals(0) && exists)
        {
            Update(entity);
        }
        else
        {
            Context.AddOrUpdateGraph(entity);
        }

        return entity;
    }

    public virtual async Task<TEntity> CreateOrUpdateAsync(
        TEntity entity,
        ICollection<Type> entitiesToBeUpdated
    )
    {
        var exists = await Exists(x => x.Id.Equals(entity.Id));
        if (entity.Id.Equals(0) && exists)
        {
            Update(entity);
        }
        else
        {
            Context.AddOrUpdateGraph(entity, entitiesToBeUpdated);
        }

        return entity;
    }

    public virtual async Task Clear()
    {
        var allEntities = await DbSet.ToListAsync();
        DbSet.RemoveRange(allEntities);
    }

    public virtual async Task DeleteByIdAsync(
        TKey id
    )
    {
        var entity = await GetOneAsync(id);
        DbSet.Remove(entity);
    }

    public virtual async Task DeleteAsync(
        TEntity entity
    )
    {
        await Task.FromResult(DbSet.Remove(entity));
    }

    public virtual async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await Context.SaveChangesAsync(cancellationToken);
    }

    protected async Task RemoveManyToManyRelationship<TOtherEntityKey>(
        string joinEntityName,
        string ownerIdKey,
        string ownedIdKey,
        TOtherEntityKey ownerEntityId,
        List<TOtherEntityKey> idsToIgnore
    )
    {
        var dbset = Context.Set<Dictionary<string, object>>(joinEntityName);

        var manyToManyData = await dbset
            .Where(joinPropertyBag => joinPropertyBag[ownerIdKey].Equals(ownerEntityId))
            .ToListAsync();

        var filteredManyToManyData = manyToManyData
            .Where(joinPropertyBag => !idsToIgnore.Any(idToIgnore => joinPropertyBag[ownedIdKey].Equals(idToIgnore)))
            .ToList();

        dbset.RemoveRange(filteredManyToManyData);
    }
}
