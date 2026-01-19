using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlueprintCqrs.Domain.Repositories.Interfaces;
using BlueprintCqrs.Infrastructure.Data.Extensions;

namespace BlueprintCqrs.Infrastructure.Data.Repositories;

public class UnitOfWork(
    DbContext context
) : IUnitOfWork
{
    protected readonly DbContext Context = context;

    public void UpdateState<TEntity>(
        TEntity entity,
        EntityState state
    )
    {
        Context.Entry(entity).State = state;
    }

    public void SetEntityStateModified<TEntiy, TProperty>(
        TEntiy entity,
        Expression<Func<TEntiy, TProperty>> propertyExpression
    ) where TEntiy : class where TProperty : class
    {
        Context.Entry(entity).Reference(propertyExpression).IsModified = true;
    }

    public void RemoveNavigationProperty<TEntity, TOwnerEntity>(
        TOwnerEntity ownerEntity,
        object id
    )
        where TEntity : class
        where TOwnerEntity : class
    {
        Context.Set<TEntity>().RemoveNavigationProperty(ownerEntity, id);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default
    )
    {
        using var saveChangeTask = Context.SaveChangesAsync(cancellationToken);
        return await saveChangeTask;
    }

    public DbSet<T> Set<T>(
        string name = null
    ) where T : class
    {
        return Context.Set<T>(name);
    }

    public void AddOrUpdateGraph<TEntiy>(
        TEntiy entity,
        ICollection<Type> entitiesToBeUpdated = null
    ) where TEntiy : class
    {
        var rootTypeEntity = entity.GetType();

        Context.ChangeTracker.TrackGraph(
            entity,
            e =>
            {
                var navigationPropertyName = e.Entry.Entity.GetType();

                var alreadyTrackedEntity = Context.ChangeTracker.Entries().FirstOrDefault(entry => entry.Entity.Equals(e.Entry.Entity));

                if (alreadyTrackedEntity != null)
                {
                    alreadyTrackedEntity.State = EntityState.Detached;
                }

                if (!navigationPropertyName.Equals(rootTypeEntity)
                    && !(entitiesToBeUpdated != null && entitiesToBeUpdated.Contains(navigationPropertyName)))
                {
                    e.Entry.State = EntityState.Unchanged;
                }
                else if (e.Entry.IsKeySet)
                {
                    e.Entry.State = EntityState.Modified;
                }
                else
                {
                    e.Entry.State = EntityState.Added;
                }

                System.Diagnostics.Debug.WriteLine($"Tracking {e.Entry.Metadata.DisplayName()} as {e.Entry.State}");
            }
        );
    }

    public void Dispose()
    {
        Context?.Dispose();
    }
}
