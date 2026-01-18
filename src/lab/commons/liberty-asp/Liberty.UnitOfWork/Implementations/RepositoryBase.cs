using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.Specification;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;

namespace Liberty.UnitOfWork.Implementations;

public class RepositoryBase<TEntity> : GenericRepository<TEntity>, IRepositoryBase<TEntity>
    where TEntity : class
{
    private readonly ILogger<RepositoryBase<TEntity>> _logger;
    private DbSet<TEntity> DbSet { get; set; }
    private readonly IAsyncPolicy _policy;

    protected RepositoryBase(
        ILogger<RepositoryBase<TEntity>> logger,
        DbContext dbContext
    ) : base(dbContext)
    {
        _logger = logger;
        DbSet = dbContext.Set<TEntity>();

        _policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                2,
                retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    + TimeSpan.FromMilliseconds(new Random().Next(0, 1000)),
                (
                    exception,
                    retryCount
                ) =>
                {
                    _logger.LogError(
                        exception,
                        "{ProcessName} - Exception retry {RetryCount}",
                        nameof(GetOneAsync),
                        retryCount
                    );
                }
            );
    }

    public IQueryable<TEntity> GetQueryable(
        ISpecification<TEntity> spec
    )
    {
        return GetQuery(DbSet, spec);
    }

    public IQueryable<TEntity> GetQueryableWithAsNoTracking(
        ISpecification<TEntity> spec
    )
    {
        return GetQuery(DbSet, spec).AsNoTracking();
    }

    public IQueryable<TEntity> GetQueryable(
        IGridSpecification<TEntity> spec
    )
    {
        return GetQuery(DbSet, spec);
    }

    public IQueryable<TEntity> GetQueryableWithAsNoTracking(
        IGridSpecification<TEntity> spec
    )
    {
        return GetQuery(DbSet, spec).AsNoTracking();
    }

    public async Task<TEntity?> GetOneAsync(
        ISpecification<TEntity> spec,
        CancellationToken cancellationToken = default
    )
    {
        return await _policy.ExecuteAsync(
            async () =>
            {
                var specificationResult = GetQuery(DbSet, spec);
                return await specificationResult.FirstOrDefaultAsync(cancellationToken);
            }
        );

        // var specificationResult = GetQuery(_dbSet, spec);
        // return await specificationResult.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>?> GetAllAsync(
        ISpecification<TEntity> spec,
        CancellationToken cancellationToken = default
    )
    {
        var specificationResult = GetQuery(DbSet, spec);
        return await specificationResult.ToListAsync(cancellationToken);
    }

    public async ValueTask<long> CountAsync(
        IGridSpecification<TEntity> spec,
        CancellationToken cancellationToken = default
    )
    {
        spec.IsPagingEnabled = false;
        var specificationResult = GetQuery(DbSet, spec);
        return await ValueTask.FromResult(
            await specificationResult.LongCountAsync(cancellationToken)
        );
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(
        IGridSpecification<TEntity> spec,
        CancellationToken cancellationToken = default
    )
    {
        var specificationResult = GetQuery(DbSet, spec);
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
                .Skip(specification.Skip - 1)
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
            var expr = specification.Criteria.First();
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
