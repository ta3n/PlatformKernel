using SharedKernel.Entity;
using SharedKernel.Pagination;

namespace SharedKernel.ServiceBase;

/// <summary>
/// A generic service interface that defines fundamental CRUD operations and utility methods
/// for entities extending the <see cref="EntityData"/> class.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity that the service will manipulate, which must derive from <see cref="EntityData"/>.
/// </typeparam>
public interface IFluentService<TEntity> where TEntity : EntityData
{
    /// <summary>
    /// Asynchronously creates a new entity of type TEntity.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of the entity to be created.
    /// </typeparam>
    /// <param name="entityToCreate">
    /// The entity instance to be created.
    /// </param>
    /// <param name="autoSave">
    /// A boolean indicating whether to automatically save changes to the database. Default value is true.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains the created entity instance.
    /// </returns>
    Task<TEntity> CreateAsync(
        TEntity entityToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously creates a range of entities in the data store.
    /// </summary>
    /// <param name="entitiesToCreate">A collection of entities to be created in the data store.</param>
    /// <param name="autoSave">Indicates whether changes should be saved automatically after the creation. Defaults to true.</param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used to cancel the operation if required. Defaults to <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>A task representing the asynchronous operation. The result contains a collection of the created entities.</returns>
    Task<IEnumerable<TEntity>> CreateRangeAsync(
        IEnumerable<TEntity> entitiesToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates an existing entity with the provided changes. Optionally saves the changes automatically
    /// or applies a custom update action to the entity.
    /// </summary>
    /// <param name="entityToUpdate">The entity to be updated.</param>
    /// <param name="autoSave">A boolean value indicating whether to save the changes automatically after the update. Defaults to true.</param>
    /// <param name="updateAction">A custom function that defines how the entity should be updated. If null, the default update logic will be used.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to observe cancellation requests.</param>
    /// <returns>A task that represents the asynchronous update operation. The task result contains the updated entity.</returns>
    Task<TEntity> UpdateAsync(
        TEntity entityToUpdate,
        bool autoSave = true,
        Func<TEntity, TEntity, TEntity>? updateAction = null,
        CancellationToken cancellationToken = default
    );

    /// Updates an entity by finding its existing version and applying an update action.
    /// <param name="entityToUpdate">The entity containing the new updated data.</param>
    /// <param name="autoSave">Indicates whether the changes should be automatically saved after the update. Default is true.</param>
    /// <param name="updateAction">A function specifying how to apply updates to the entity. It takes the existing entity and the new entity as input and returns the updated entity. Default is null.</param>
    /// <param name="findByIdAction">A function to retrieve the existing entity based on its identifier. Default is null.</param>
    /// <param name="cancellationToken">The cancellation token to observe while awaiting the operation. Default is CancellationToken.None.</param>
    /// <returns>Returns the updated entity after applying the changes.</returns>
    Task<TEntity> UpdateWithFindByActionAsync(
        TEntity entityToUpdate,
        bool autoSave = true,
        Func<TEntity, TEntity, TEntity>? updateAction = null,
        Func<long, Task<TEntity>>? findByIdAction = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates a range of entities in the data source.
    /// </summary>
    /// <param name="entitiesToUpdate">The collection of entities to update.</param>
    /// <param name="autoSave">Indicates whether changes should be saved automatically after the update. Defaults to true.</param>
    /// <param name="updateAction">
    /// An optional function to define how each entity should be updated.
    /// If provided, this function takes the old and updated entity as arguments and returns the updated entity.
    /// </param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the updated entities.</returns>
    Task<IEnumerable<TEntity>> UpdateRangeAsync(
        IEnumerable<TEntity> entitiesToUpdate,
        bool autoSave = true,
        Func<TEntity, TEntity, TEntity>? updateAction = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Enables or disables an entity identified by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to enable or disable.</param>
    /// <param name="isEnabled">Specifies whether the entity should be enabled or disabled.</param>
    /// <param name="autoSave">Indicates whether changes should be saved automatically. Defaults to true.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete. Defaults to None.</param>
    /// <returns>A task representing the asynchronous operation, which returns the updated entity.</returns>
    Task<TEntity> EnableAsync(
        long id,
        bool isEnabled,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Arranges a series of swaps between entities based on the provided swap steps.
    /// </summary>
    /// <param name="swapSteps">
    /// A list of tuples where each tuple represents a swap operation
    /// containing the source entity ID and the destination entity ID.
    /// </param>
    /// <param name="cancellationToken">
    /// A CancellationToken to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// an IEnumerable of entities after the swap operations are performed.
    /// </returns>
    Task<IEnumerable<TEntity>> ArrangeSwapAsync(
        List<(long sourceId, long destinationId)> swapSteps,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates the order of entities based on the provided list of identifiers.
    /// </summary>
    /// <param name="ids">A list of unique identifiers representing the new order of entities.</param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, typically to stop the asynchronous operation.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ArrangeOrderAsync(
        List<long> ids,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Deletes an entity with the specified identifier and optionally saves the changes automatically.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <param name="autoSave">Determines whether changes should be saved automatically after deletion. Default is true.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The deleted entity.</returns>
    Task<TEntity> DeleteAsync(
        long id,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    /// Deletes a range of entities identified by their IDs asynchronously.
    /// <param name="ids">
    /// An array of IDs representing the entities to be deleted.
    /// </param>
    /// <param name="autoSave">
    /// Indicates whether changes should be automatically saved after the deletion.
    /// Defaults to true.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the operation to complete.
    /// Defaults to CancellationToken.None.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains a collection
    /// of the deleted entities.
    /// </returns>
    Task<IEnumerable<TEntity>> DeleteRangeAsync(
        long[] ids,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Deletes the specified entity physically from the data store based on its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to be deleted.</param>
    /// <param name="autoSave">Determines whether changes are automatically committed to the data store. Defaults to true.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeletePhysicalAsync(
        long id,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Deletes a range of physical entities from the database permanently.
    /// </summary>
    /// <param name="ids">An array of entity IDs to be deleted.</param>
    /// <param name="autoSave">A boolean flag indicating whether changes should be auto-saved to the database.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeletePhysicalRangeAsync(
        long[] ids,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously retrieves a paginated collection of entities.
    /// </summary>
    /// <param name="pageable">An object that defines pagination and sorting information.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a paginated collection of entities of type <typeparamref name="TEntity"/>.</returns>
    Task<IPage<TEntity>> FindAllAsync(
        IPageable pageable,
        CancellationToken cancellationToken = default
    );

    /// Finds and retrieves a collection of entities based on the provided array of IDs.
    /// <param name="ids">The array of unique identifiers representing the entities to be retrieved.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing a collection of entities that match the specified IDs.</returns>
    Task<IEnumerable<TEntity>> FindAllByIdsAsync(
        long[] ids,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously retrieves an entity of type <typeparamref name="TEntity"/> by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <param name="cancellationToken">
    /// An optional cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the entity of type <typeparamref name="TEntity"/> if found; otherwise, null.
    /// </returns>
    Task<TEntity> FindByIdAsync(
        long id,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously counts the number of entities corresponding to the provided list of identifiers.
    /// </summary>
    /// <param name="ids">An array of identifiers representing the entities to be counted.</param>
    /// <param name="cancellationToken">Optional. A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the count of entities corresponding to the provided identifiers.</returns>
    Task<int> CountByIdsAsync(
        long[] ids,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously counts the number of entities matching the specified codes.
    /// </summary>
    /// <param name="codes">An array of codes to filter the entities.</param>
    /// <param name="cancellationToken">
    /// A token to observe while waiting for the task to complete, or to cancel the operation.
    /// </param>
    /// <returns>The total count of entities that match the specified codes.</returns>
    Task<int> CountByCodesAsync(
        string[] codes,
        CancellationToken cancellationToken = default
    );
}
