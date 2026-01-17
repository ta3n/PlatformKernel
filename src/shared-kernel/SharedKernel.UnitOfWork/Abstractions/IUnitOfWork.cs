using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;

namespace SharedKernel.UnitOfWork.Abstractions;

/// <summary>
/// Represents a contract for a Unit of Work pattern abstraction to manage database transactions,
/// entities, and their states within an application.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// Provides access to a `DbSet` for the specified entity type.
    /// <typeparam name="T">The type of the entity for which the `DbSet` is being retrieved.</typeparam>
    /// <returns>
    /// A `DbSet` instance used to perform CRUD operations and queries for the given entity type.
    /// </returns>
    DbSet<T> Set<T>() where T : class;

    /// <summary>
    /// Provides access to the underlying database connection used within the unit of work.
    /// The connection is created on demand using a connection factory if it does not
    /// already exist.
    /// </summary>
    /// <remarks>
    /// The <see cref="Connection"/> property can be used to execute raw database queries
    /// or commands that are not directly supported by the Entity Framework. This allows
    /// fine-grained control over database operations when necessary.
    /// If a connection factory is specified during the initialization of the unit of work,
    /// the connection is lazily instantiated using the provided factory method. If no factory
    /// is specified, the connection object remains null unless otherwise managed.
    /// </remarks>
    IDbConnection? Connection { get; }

    /// <summary>
    /// Begins a new transaction asynchronously with a specified isolation level and cancellation token.
    /// </summary>
    /// <param name="isolationLevel">
    /// The isolation level of the transaction. Defaults to <see cref="IsolationLevel.Unspecified"/> if not provided.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> instance that can be used to observe cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of starting a transaction.
    /// </returns>
    Task BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.Unspecified,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Commits all changes made to the underlying data source as part of the current unit of work.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing cooperative cancellation of the operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous commit operation.
    /// </returns>
    Task CommitAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Reverts any uncommitted changes within the unit of work, ensuring no changes are persisted to the data store.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task that represents the asynchronous rollback operation.</return>
    Task RollbackAsync(
        CancellationToken cancellationToken = default
    );

    /// Saves all changes made in the context to the database.
    /// <returns>
    /// The number of state entries written to the database.
    /// </returns>
    int SaveChanges();

    /// <summary>
    /// Asynchronously saves changes made in the context to the database.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.
    /// </returns>
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default
    );

    /// Updates the state of the specified entity in the database context.
    /// <typeparam name="TEntity">The type of the entity to update.</typeparam>
    /// <param name="entity">The entity whose state needs to be updated. Can be null.</param>
    /// <param name="state">The new state to assign to the entity.</param>
    void UpdateState<TEntity>(
        TEntity? entity,
        EntityState state
    );

    /// <summary>
    /// Sets the state of a specified entity to "Modified" for a given property in the entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to be modified.</typeparam>
    /// <typeparam name="TProperty">The type of the property to be marked as modified.</typeparam>
    /// <param name="entity">The entity instance whose state is to be modified. Can be null.</param>
    /// <param name="propertyExpression">
    /// An expression that specifies the property of the entity to be marked as modified. Can be null.
    /// </param>
    void SetEntityStateModified<TEntity, TProperty>(
        TEntity? entity,
        Expression<Func<TEntity, TProperty>>? propertyExpression
    ) where TEntity : class where TProperty : class;

    /// Retrieves the instance of the DbContext associated with the Unit of Work implementation.
    /// This method provides access to the underlying DbContext for further operations or interactions.
    /// <returns>
    /// The instance of Microsoft.EntityFrameworkCore.DbContext.
    /// </returns>
    DbContext GetDbContext();
}
