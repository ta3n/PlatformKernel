using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.Specification;

namespace Liberty.UnitOfWork.Abstractions;

public interface IRepositoryBase<TEntity>
    : IGenericRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> GetQueryable(
        ISpecification<TEntity> spec
    );

    IQueryable<TEntity> GetQueryableWithAsNoTracking(
        ISpecification<TEntity> spec
    );

    IQueryable<TEntity> GetQueryable(
        IGridSpecification<TEntity> spec
    );

    IQueryable<TEntity> GetQueryableWithAsNoTracking(
        IGridSpecification<TEntity> spec
    );

    Task<TEntity?> GetOneAsync(
        ISpecification<TEntity> spec,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<TEntity>?> GetAllAsync(
        ISpecification<TEntity> spec,
        CancellationToken cancellationToken = default
    );

    ValueTask<long> CountAsync(
        IGridSpecification<TEntity> spec,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<TEntity>> GetAllAsync(
        IGridSpecification<TEntity> spec,
        CancellationToken cancellationToken = default
    );
}
