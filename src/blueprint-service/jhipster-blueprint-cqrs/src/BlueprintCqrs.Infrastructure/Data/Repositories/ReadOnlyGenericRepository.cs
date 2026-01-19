using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using JHipsterNet.Core.Pagination.Extensions;
using Microsoft.EntityFrameworkCore;
using BlueprintCqrs.Domain.Repositories.Interfaces;
using BlueprintCqrs.Domain.Entities;

namespace BlueprintCqrs.Infrastructure.Data.Repositories;

public abstract class ReadOnlyGenericRepository<TEntity, TKey>(
    IUnitOfWork context
) : IReadOnlyGenericRepository<TEntity, TKey>, IDisposable where TEntity : BaseEntity<TKey>
{
    protected internal readonly IUnitOfWork Context = context;
    protected internal readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

    public virtual async Task<TEntity> GetOneAsync(
        TKey id
    )
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public virtual async Task<IPage<TEntity>> GetPageAsync(
        IPageable pageable
    )
    {
        return await DbSet.UsePageableAsync(pageable);
    }

    public virtual async Task<bool> Exists(
        Expression<Func<TEntity, bool>> predicate
    )
    {
        return await DbSet.AnyAsync(predicate);
    }

    public virtual async Task<int> CountAsync()
    {
        var countTask = await DbSet.CountAsync();
        return countTask;
    }

    public virtual IFluentRepository<TEntity> QueryHelper()
    {
        var fluentRepository = new FluentRepository<TEntity>(DbSet);
        return fluentRepository;
    }

    public void Dispose()
    {
        Context?.Dispose();
    }
}
