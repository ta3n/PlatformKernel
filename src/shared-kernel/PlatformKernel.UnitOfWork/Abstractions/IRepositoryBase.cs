using PlatformKernel.ApplicationShared.Domains.Repositories;
using PlatformKernel.Specification;

namespace PlatformKernel.UnitOfWork.Abstractions;

/// <summary>
/// Provides an abstraction for a repository with additional methods for querying entities
/// using specifications or grid specifications. This interface extends
/// <see cref="IGenericRepository{TEntity}"/> for basic CRUD operations.
/// </summary>
/// <typeparam name="TEntity">The type of entity that the repository operates on.</typeparam>
public interface IRepositoryBase<TEntity>
    : IGenericRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Retrieves a queryable collection of entities that match the given specification.
    /// </summary>
    /// <param name="spec">The specification used to filter and shape the data.</param>
    /// <returns>An <see cref="IQueryable{TEntity}"/> collection filtered and shaped as per the provided specification.</returns>
    IQueryable<TEntity> GetQueryable(
        ISpecification<TEntity> spec
    );

    /// <summary>
    /// Retrieves an <see cref="IQueryable{TEntity}"/> based on the provided grid specification.
    /// </summary>
    /// <param name="spec">The grid specification used to filter, include, and configure the query.</param>
    /// <returns>An <see cref="IQueryable{TEntity}"/> representing the query result based on the specified criteria.</returns>
    IQueryable<TEntity> GetQueryable(
        IGridSpecification<TEntity> spec
    );

    /// <summary>
    /// Retrieves a queryable collection of entities based on the given specification
    /// with the `AsNoTracking` behavior applied for read-only access.
    /// </summary>
    /// <param name="spec">The specification that contains the criteria and conditions
    /// for filtering and projecting the data.</param>
    /// <returns>A queryable collection of entities of type <typeparamref name="TEntity"/>
    /// which have not been tracked by the context.</returns>
    IQueryable<TEntity> GetQueryableWithAsNoTracking(
        ISpecification<TEntity> spec
    );

    /// <summary>
    /// Retrieves an <see cref="IQueryable{T}"/> of entities that match the given <see cref="IGridSpecification{T}"/>,
    /// ensuring the query is executed without tracking the retrieved entities.
    /// This is useful for read-only operations where tracking is not necessary, improving performance.
    /// </summary>
    /// <param name="spec">The grid specification defining the filtering, sorting, and other query behaviors for the operation.</param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> of entities that match the specified grid specification,
    /// executed without tracking changes to the entities.
    /// </returns>
    IQueryable<TEntity> GetQueryableWithAsNoTracking(
        IGridSpecification<TEntity> spec
    );

    /// <summary>
    /// Asynchronously retrieves the first entity that matches the given specification.
    /// </summary>
    /// <param name="spec">
    /// The specification containing the criteria for filtering the entities.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to observe while waiting for the task to complete. Defaults to <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the first entity that matches the specification,
    /// or <c>null</c> if no such entity exists.
    /// </returns>
    Task<TEntity?> GetOneAsync(
        ISpecification<TEntity> spec,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously counts the number of entities matching the specified grid specification.
    /// </summary>
    /// <param name="spec">The grid specification used to filter the entities.</param>
    /// <param name="cancellationToken">The token used to propagate notification that the operation should be canceled.</param>
    /// <returns>The total number of entities that match the given grid specification.</returns>
    ValueTask<long> CountAsync(
        IGridSpecification<TEntity> spec,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves all entities that satisfy the specified criteria in the given specification.
    /// </summary>
    /// <param name="spec">The specification defining the query criteria and configurations.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, which contains a read-only list of entities matching the specification or null if none are found.</returns>
    Task<IReadOnlyList<TEntity>?> GetAllAsync(
        ISpecification<TEntity> spec,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves all entities that match the specified criteria.
    /// </summary>
    /// <param name="spec">The specification containing the filtering, sorting, and pagination information to apply when querying the entities.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete, used to cancel the operation if required.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of entities that match the criteria applied.</returns>
    Task<IReadOnlyList<TEntity>> GetAllAsync(
        IGridSpecification<TEntity> spec,
        CancellationToken cancellationToken = default
    );
}
