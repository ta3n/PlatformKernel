using Microsoft.EntityFrameworkCore;

namespace SharedKernel.RepositoryBase;

/// <summary>
/// Represents a generic repository interface for basic CRUD operations and data persistence handling.
/// </summary>
/// <typeparam name="T">
/// The type of the entity for which this repository is responsible. It must be a class.
/// </typeparam>
public interface IGenericRepository<T> where T : class
{
    /// Returns an IQueryable representing the database queryable for the entity type T.
    /// This method provides the ability to query the database for entities of the specified type.
    /// It can be used to construct and execute LINQ queries against the underlying data store.
    /// <returns>
    /// An IQueryable instance for the entity type T.
    /// </returns>
    IQueryable<T> GetQueryable();

    /// <summary>
    /// Retrieves an <see cref="IQueryable{T}"/> from the repository that is configured with
    /// the "AsNoTracking" option enabled, which means the returned objects are not tracked
    /// by the context, potentially improving query performance for read-only operations.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="IQueryable{T}"/> with "AsNoTracking" applied for entities
    /// of the specified generic type <typeparamref name="T"/>.
    /// </returns>
    IQueryable<T> GetQueryableWithAsNoTracking();

    /// <summary>
    /// Retrieves a queryable of entities with the AsNoTracking option applied.
    /// This allows entities to be tracked without being cached by the DbContext,
    /// providing read-only behavior and improving query performance in scenarios where
    /// tracking is unnecessary.
    /// </summary>
    /// <param name="appDbContext">The database context to execute the query against.</param>
    /// <returns>An <see cref="IQueryable{T}"/> of entities with AsNoTracking applied.</returns>
    IQueryable<T> GetQueryableWithAsNoTracking(
        DbContext appDbContext
    );

    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains the entity with the specified identifier, or null if no matching entity is found.
    /// </returns>
    Task<T?> GetByIdAsync(
        long id
    );

    /// <summary>
    /// Retrieves all entities of type <typeparamref name="T"/> from the repository.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of all entities of type <typeparamref name="T"/> in the repository.</returns>
    Task<IReadOnlyList<T>> GetAllAsync();

    /// <summary>
    /// Retrieves a paginated list of entities based on the specified page number and size.
    /// </summary>
    /// <param name="pageNumber">The number of the current page to fetch. Starts from 1.</param>
    /// <param name="pageSize">The number of items to include in a single page.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a read-only list of entities corresponding to the requested page.</returns>
    Task<IReadOnlyList<T>> GetPagedAsync(
        int pageNumber,
        int pageSize
    );

    /// Saves all changes made in the current context to the database.
    /// <returns>
    /// The number of state entries written to the database.
    /// </returns>
    int SaveChanges();

    /// Asynchronously saves all changes made in the context to the database.
    /// This operation commits any modifications made to the tracked entities.
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the operation to complete.</param>
    /// <return>Returns the number of state entries written to the database.</return>
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Adds a new entity to the database context.
    /// </summary>
    /// <param name="entity">The entity to be added to the database.</param>
    /// <param name="autoSave">
    /// Determines whether changes should be automatically persisted to the database after adding the entity.
    /// If true, the changes are saved automatically; otherwise, the changes must be saved explicitly.
    /// </param>
    /// <returns>The added entity.</returns>
    T Add(
        T entity,
        bool autoSave = false
    );

    /// Asynchronously adds a new entity to the repository and optionally saves changes to the underlying database.
    /// <param name="entity">The entity to be added to the repository.</param>
    /// <param name="autoSave">
    /// A boolean flag indicating whether to automatically save changes to the database after adding the entity.
    /// Defaults to false if not specified.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests. Defaults to CancellationToken.None if not specified.
    /// </param>
    /// <return>Returns the added entity.</return>
    Task<T> AddAsync(
        T entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Adds a collection of entities to the data context and optionally saves the changes.
    /// </summary>
    /// <param name="entities">The collection of entities to be added.</param>
    /// <param name="autoSave">Specifies whether changes should be automatically saved to the database. Defaults to false.</param>
    /// <returns>The collection of entities that were added.</returns>
    IEnumerable<T> AddRange(
        IEnumerable<T> entities,
        bool autoSave = false
    );

    /// Asynchronously adds a collection of entities to the repository.
    /// <param name="entities">
    /// A collection of entities of type T to be added to the repository.
    /// </param>
    /// <param name="autoSave">
    /// A boolean flag indicating whether the changes should be automatically saved to the database. Defaults to false.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used to cancel the asynchronous operation. Defaults to CancellationToken.None.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing the added entities as an enumerable collection.
    /// </returns>
    Task<IEnumerable<T>> AddRangeAsync(
        IEnumerable<T> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    );

    /// Updates an existing entity in the underlying data store and marks it as modified.
    /// <param name="entity">The entity to be updated.</param>
    /// <param name="autoSave">
    /// A flag indicating whether changes should be persisted automatically after the update.
    /// If true, changes are saved to the database; otherwise, changes are not immediately persisted.
    /// </param>
    /// <returns>The updated entity.</returns>
    T Update(
        T entity,
        bool autoSave = false
    );

    /// <summary>
    /// Updates the specified entity in the repository. Optionally saves changes depending on the autoSave parameter.
    /// </summary>
    /// <typeparam name="T">The type of the entity being updated.</typeparam>
    /// <param name="entity">The entity to be updated.</param>
    /// <param name="autoSave">Indicates whether to automatically save changes after updating. Default is false.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. Default is CancellationToken.None.</param>
    /// <returns>A task that represents the asynchronous update operation. The task result contains the updated entity.</returns>
    Task<T> UpdateAsync(
        T entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates a collection of entities in the database.
    /// </summary>
    /// <param name="entities">The collection of entities to be updated.</param>
    /// <param name="autoSave">Indicates whether to save changes to the database automatically after the update. Default is false.</param>
    /// <returns>Returns the updated collection of entities.</returns>
    IEnumerable<T> UpdateRange(
        IEnumerable<T> entities,
        bool autoSave = false
    );

    /// <summary>
    /// Updates a range of entities in the repository.
    /// </summary>
    /// <param name="entities">
    /// The collection of entities to be updated.
    /// </param>
    /// <param name="autoSave">
    /// Specifies whether changes should be saved automatically after updating the entities. Default is false.
    /// </param>
    /// <param name="cancellationToken">
    /// A CancellationToken used to observe cancellation requests while performing the operation. Default is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, returning the updated entities after the operation completes.
    /// </returns>
    Task<IEnumerable<T>> UpdateRangeAsync(
        IEnumerable<T> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Deletes the specified entity from the database.
    /// </summary>
    /// <param name="entity">The entity to be deleted.</param>
    /// <param name="autoSave">Indicates whether to automatically save changes after deleting the entity. Default is false.</param>
    void Delete(
        T entity,
        bool autoSave = false
    );

    /// <summary>
    /// Deletes the specified entity from the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to be deleted.</param>
    /// <param name="autoSave">
    /// A boolean flag indicating whether changes should be automatically saved to the database.
    /// Defaults to false.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests. Defaults to CancellationToken.None.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task DeleteAsync(
        T entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Deletes a range of entities from the database context and optionally saves changes immediately.
    /// </summary>
    /// <param name="entities">The collection of entities to be deleted.</param>
    /// <param name="autoSave">
    /// A boolean flag indicating whether changes should be automatically saved to
    /// the database after the deletion operation.
    /// </param>
    void DeleteRange(
        IEnumerable<T> entities,
        bool autoSave = false
    );

    /// <summary>
    /// Asynchronously deletes a range of entities from the repository.
    /// </summary>
    /// <param name="entities">
    /// A collection of entities to be deleted.
    /// </param>
    /// <param name="autoSave">
    /// A boolean value indicating whether changes to the database should be automatically saved
    /// after the deletion. The default value is <c>false</c>.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="System.Threading.CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.
    /// </returns>
    Task DeleteRangeAsync(
        IEnumerable<T> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    );
}
