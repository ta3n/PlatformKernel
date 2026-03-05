using Liberty.Entity;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

/// <summary>
/// Defines a base service interface for handling operations related to entities of type <typeparamref name="TEntity"/>.
/// </summary>
/// <typeparam name="TEntity">The type of entity managed by this service, constrained to inherit from <see cref="EntityRelation"/>.</typeparam>
public interface IBaseServiceRelation<TEntity> where TEntity : EntityRelation
{
    /// <summary>
    /// Creates a new entity of type TEntity in the repository and optionally saves changes.
    /// </summary>
    /// <param name="entityToCreate">The entity to create.</param>
    /// <param name="autoSave">Indicates whether changes should be automatically saved to the repository. Default is true.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. Default is none.</param>
    /// <returns>Returns the created entity of type TEntity after successful creation.</returns>
    Task<TEntity> CreateAsync(
        TEntity entityToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously creates a range of entities and optionally saves the changes to the repository.
    /// </summary>
    /// <param name="entitiesToCreate">
    /// A collection of entities to be created.
    /// </param>
    /// <param name="autoSave">
    /// A boolean value indicating whether the changes should be automatically saved to the repository. The default value is true.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the collection of created entities.
    /// </returns>
    Task<IEnumerable<TEntity>> CreateRangeAsync(
        IEnumerable<TEntity> entitiesToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates the specified entity in the data store. Optionally commits the changes immediately.
    /// </summary>
    /// <param name="entityToUpdate">The entity to be updated.</param>
    /// <param name="autoSave">Indicates whether changes should be automatically saved. Defaults to true.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The updated entity.</returns>
    Task<TEntity> UpdateAsync(
        TEntity entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    /// Updates a range of entities asynchronously in the repository.
    /// <param name="entitiesToUpdate">
    /// The collection of entities to be updated in the repository.
    /// </param>
    /// <param name="autoSave">
    /// Indicates whether changes should automatically be saved to the database. Defaults to true.
    /// </param>
    /// <param name="cancellationToken">
    /// A CancellationToken to observe while waiting for the operation to complete. Defaults to CancellationToken.None.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation and contains the updated entities as a result.
    /// </returns>
    Task<IEnumerable<TEntity>> UpdateRangeAsync(
        IEnumerable<TEntity> entitiesToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    /// Deletes the specified entity asynchronously from the underlying data source.
    /// <param name="entityToDelete">The entity to be deleted.</param>
    /// <param name="autoSave">
    /// A boolean value indicating whether the changes should be saved automatically after the deletion.
    /// Defaults to true if not specified.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the asynchronous operation. Defaults to CancellationToken.None.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the deleted entity.
    /// </returns>
    Task<TEntity> DeleteAsync(
        TEntity entityToDelete,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Deletes a collection of entities of type <typeparamref name="TEntity"/> in a batch operation.
    /// </summary>
    /// <param name="entitiesToDelete">
    /// A collection of entities to be deleted from the data source.
    /// </param>
    /// <param name="autoSave">
    /// Indicates whether to automatically save changes after the deletion. Default is <c>true</c>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete. Default is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the enumerable collection of entities that were deleted.
    /// </returns>
    Task<IEnumerable<TEntity>> DeleteRangeAsync(
        IEnumerable<TEntity> entitiesToDelete,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );
}
