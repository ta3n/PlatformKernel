using Microsoft.EntityFrameworkCore;
using PlatformKernel.ApplicationShared.Domains.Repositories;
using PlatformKernel.Entity;
using PlatformKernel.Specification;
using PlatformKernel.UnitOfWork.Abstractions;

namespace PlatformKernel.UnitOfWork.Implementations;

public class RepositoryBase<TEntity> : GenericRepository<TEntity>, IRepositoryBase<TEntity>
    where TEntity : class, IBaseEntity
{
    private DbContext DbContext { get; }

    protected RepositoryBase(
        DbContext dbContext
    ) : base(dbContext)
    {
        DbContext = dbContext;
    }

    public override IQueryable<TEntity> GetQueryable()
    {
        return base.GetQueryable().OrderByDescending(x => x.DisplayOrder);
    }

    public override IQueryable<TEntity> GetQueryableWithAsNoTracking()
    {
        return base.GetQueryableWithAsNoTracking().OrderByDescending(x => x.DisplayOrder);
    }

    public override IQueryable<TEntity> GetQueryableWithAsNoTracking(
        DbContext appDbContext
    )
    {
        return base.GetQueryableWithAsNoTracking(appDbContext).OrderByDescending(x => x.DisplayOrder);
    }

    public IQueryable<TEntity> GetQueryable(
        ISpecification<TEntity> spec
    )
    {
        return GetQuery(DbContext.Set<TEntity>(), spec);
    }

    public IQueryable<TEntity> GetQueryable(
        IGridSpecification<TEntity> spec
    )
    {
        return GetQuery(DbContext.Set<TEntity>(), spec);
    }

    public IQueryable<TEntity> GetQueryableWithAsNoTracking(
        ISpecification<TEntity> spec
    )
    {
        return GetQuery(DbContext.Set<TEntity>(), spec).AsNoTracking();
    }

    public IQueryable<TEntity> GetQueryableWithAsNoTracking(
        IGridSpecification<TEntity> spec
    )
    {
        return GetQuery(DbContext.Set<TEntity>(), spec).AsNoTracking();
    }

    public async Task<TEntity?> GetOneAsync(
        ISpecification<TEntity> spec,
        CancellationToken cancellationToken = default
    )
    {
        var specificationResult = GetQuery(DbContext.Set<TEntity>(), spec);
        return await specificationResult.FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<long> CountAsync(
        IGridSpecification<TEntity> spec,
        CancellationToken cancellationToken = default
    )
    {
        spec.IsPagingEnabled = false;
        var specificationResult = GetQuery(DbContext.Set<TEntity>(), spec);
        return await ValueTask.FromResult(
            await specificationResult.LongCountAsync(cancellationToken)
        );
    }

    public async Task<IReadOnlyList<TEntity>?> GetAllAsync(
        ISpecification<TEntity> spec,
        CancellationToken cancellationToken = default
    )
    {
        var specificationResult = GetQuery(DbContext.Set<TEntity>(), spec);
        return await specificationResult.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(
        IGridSpecification<TEntity> spec,
        CancellationToken cancellationToken = default
    )
    {
        var specificationResult = GetQuery(DbContext.Set<TEntity>(), spec);
        return await specificationResult.ToListAsync(cancellationToken);
    }

    private static IQueryable<TEntity> GetQuery(
        IQueryable<TEntity> inputQuery,
        ISpecification<TEntity> specification
    )
    {
        var query = inputQuery;

        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        query = specification.Includes.Aggregate(
            query,
            (
                current,
                include
            ) => current.Include(include)
        );

        query = specification.IncludeStrings.Aggregate(
            query,
            (
                current,
                include
            ) => current.Include(include)
        );

        if (specification.OrderBy is not null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending is not null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        if (specification.GroupBy is not null)
        {
            query = query
                .GroupBy(specification.GroupBy)
                .SelectMany(x => x);
        }

        if (specification.IsPagingEnabled)
        {
            query = query
                .Skip(Math.Max(0, specification.Skip - 1))
                .Take(specification.Take);
        }

        query = query.AsSplitQuery();

        return query;
    }

    private static IQueryable<TEntity> GetQuery(
        IQueryable<TEntity> inputQuery,
        IGridSpecification<TEntity> specification
    )
    {
        var query = inputQuery;

        if (specification.Criteria is { Count: > 0 })
        {
            var expr = specification.Criteria[0];
            for (var i = 1; i < specification.Criteria.Count; i++)
            {
                expr = expr.And(specification.Criteria[i]);
            }

            query = query.Where(expr);
        }

        query = specification.Includes.Aggregate(
            query,
            (
                current,
                include
            ) => current.Include(include)
        );

        query = specification.IncludeStrings.Aggregate(
            query,
            (
                current,
                include
            ) => current.Include(include)
        );

        if (specification.OrderBy is not null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending is not null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        if (specification.GroupBy is not null)
        {
            query = query
                .GroupBy(specification.GroupBy)
                .SelectMany(x => x);
        }

        if (specification.IsPagingEnabled)
        {
            query = query
                .Skip(specification.Skip - 1)
                .Take(specification.Take);
        }

        query = query.AsSplitQuery();

        return query;
    }
}
