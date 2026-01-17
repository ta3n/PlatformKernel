using Blueprint.Service.Base.Application.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedKernel.Cache.Services;
using SharedKernel.Cache.Utils;
using SharedKernel.Entity;
using SharedKernel.Pagination;
using SharedKernel.Pagination.Extensions;
using SharedKernel.UnitOfWork.Abstractions;

namespace Blueprint.Service.Base.Application.Domain.Service;

public class BaseService<TEntity>
    : IBaseService<TEntity>
    where TEntity : EntityData
{
    private readonly ILogger _logger;
    private readonly IRepositoryBase<TEntity> _entityRepository;
    private readonly Exception _appNotfoundException;

    protected readonly ICacheService? CacheService;

    protected BaseService(
        ILogger logger,
        IRepositoryBase<TEntity> entityRepository,
        Exception appNotfoundException
    )
    {
        _logger = logger;
        _entityRepository = entityRepository;
        _appNotfoundException = appNotfoundException;
    }

    protected BaseService(
        ILogger logger,
        ICacheService cacheService,
        IRepositoryBase<TEntity> entityRepository,
        Exception appNotfoundException
    )
    {
        _logger = logger;
        CacheService = cacheService;
        _entityRepository = entityRepository;
        _appNotfoundException = appNotfoundException;
    }

    protected virtual string GetCacheKey(
        string methodName = "",
        params string[] keys
    )
    {
        var hashKey = CacheHelper.ComputeHash(
            string.IsNullOrEmpty(methodName)
                ? keys
                : [methodName, .. keys]
        );

        return $"{typeof(TEntity).Name}:{hashKey}";
    }

    public virtual async Task<TEntity> CreateAsync(
        TEntity entityToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var addEntity = await _entityRepository.AddAsync(
            entityToCreate,
            autoSave,
            cancellationToken
        );

        _logger.LogInformation(
            "{BaseServiceName}.{AsyncName} - Successfully created new {EntityDataName} {EntityAddId}",
            nameof(BaseService<TEntity>),
            nameof(CreateRangeAsync),
            nameof(TEntity),
            addEntity.Id
        );

        return addEntity;
    }

    public virtual async Task<IEnumerable<TEntity>> CreateRangeAsync(
        IEnumerable<TEntity> entitiesToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var addEntities = await _entityRepository.AddRangeAsync(
            entitiesToCreate,
            autoSave,
            cancellationToken
        );

        var entityData = addEntities as TEntity[] ?? [.. addEntities];
        _logger.LogInformation(
            "{BaseServiceName}.{AsyncName} - Successfully created new {EntityDataName} {Count}",
            nameof(BaseService<TEntity>),
            nameof(CreateRangeAsync),
            nameof(TEntity),
            entityData.Length
        );

        return entityData;
    }

    public virtual async Task<TEntity> UpdateAsync(
        TEntity entityToUpdate,
        bool autoSave = true,
        Func<TEntity, TEntity, TEntity>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var existingEntity = await FindByIdAsync(entityToUpdate.Id, cancellationToken) ?? throw _appNotfoundException;
        existingEntity = updateAction?.Invoke(existingEntity, entityToUpdate) ?? throw _appNotfoundException;

        var entityEdit = await _entityRepository.UpdateAsync(
            existingEntity,
            autoSave,
            cancellationToken
        );

        _logger.LogInformation(
            "{BaseServiceName}.{UpdateAsyncName} - Successfully updated {EntityDataName} {EntityEditId}",
            nameof(BaseService<TEntity>),
            nameof(UpdateRangeAsync),
            nameof(TEntity),
            entityEdit.Id
        );

        return entityEdit;
    }

    public async Task<TEntity> UpdateWithFindByActionAsync(
        TEntity entityToUpdate,
        bool autoSave = true,
        Func<TEntity, TEntity, TEntity>? updateAction = null,
        Func<long, Task<TEntity>>? findByIdAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var existingEntity = (findByIdAction is null
                ? await FindByIdAsync(entityToUpdate.Id, cancellationToken)
                : await findByIdAction.Invoke(entityToUpdate.Id))
            ?? throw _appNotfoundException;
        existingEntity = updateAction?.Invoke(existingEntity, entityToUpdate) ?? throw _appNotfoundException;

        var entityEdit = await _entityRepository.UpdateAsync(
            existingEntity,
            autoSave,
            cancellationToken
        );

        _logger.LogInformation(
            "{BaseServiceName}.{UpdateAsyncName} - Successfully updated {EntityDataName} {EntityEditId}",
            nameof(BaseService<TEntity>),
            nameof(UpdateRangeAsync),
            nameof(TEntity),
            entityEdit.Id
        );

        return entityEdit;
    }

    public virtual async Task<IEnumerable<TEntity>> UpdateRangeAsync(
        IEnumerable<TEntity> entitiesToUpdate,
        bool autoSave = true,
        Func<TEntity, TEntity, TEntity>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var entityData = entitiesToUpdate as TEntity[] ?? [.. entitiesToUpdate];
        var ids = entityData.Select(x => x.Id).ToArray();
        var existingEntities = await GetQueryable()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
        if (existingEntities.Count != entityData.Length)
        {
            throw _appNotfoundException;
        }

        for (var i = 0; i < entityData.Length; i++)
        {
            var existingEntity = existingEntities.Find(x => x.Id == entityData[i].Id);
            if (existingEntity is null)
            {
                continue;
            }

            entityData[i] = updateAction?.Invoke(existingEntity, entityData[i]) ?? throw _appNotfoundException;
        }

        var editEntities = (await _entityRepository.UpdateRangeAsync(
            entityData,
            autoSave,
            cancellationToken
        )).ToList();

        _logger.LogInformation(
            "{BaseServiceName}.{UpdateAsyncName} - Successfully updated {EntityDataName} {Count}",
            nameof(BaseService<TEntity>),
            nameof(UpdateRangeAsync),
            nameof(TEntity),
            editEntities.Count
        );

        return editEntities;
    }

    public virtual async Task<TEntity> EnableAsync(
        long id,
        bool isEnabled,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var existingEntity = await FindByIdAsync(id, cancellationToken);
        existingEntity.IsEnabled = isEnabled;

        var entityEdit = await _entityRepository.UpdateAsync(
            existingEntity,
            autoSave,
            cancellationToken
        );

        _logger.LogInformation(
            "{BaseServiceName}.{UpdateAsyncName} - Successfully updated {EntityDataName} {EntityEditId}",
            nameof(BaseService<TEntity>),
            nameof(EnableAsync),
            nameof(TEntity),
            entityEdit.Id
        );

        return entityEdit;
    }

    public virtual async Task<IEnumerable<TEntity>> ArrangeSwapAsync(
        List<(long sourceId, long destinationId)> swapSteps,
        CancellationToken cancellationToken = default
    )
    {
        var ids = swapSteps
            .SelectMany(x => new[] { x.sourceId, x.destinationId })
            .Distinct()
            .ToList();

        var existingEntities = await GetQueryable()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
        if (existingEntities.Count != ids.Count)
        {
            throw _appNotfoundException;
        }

        var sourceDestinationDictionary = new Dictionary<long, long>();

        foreach (var (sourceId, destinationId) in swapSteps)
        {
            if (!sourceDestinationDictionary.TryGetValue(sourceId, out _))
            {
                sourceDestinationDictionary[sourceId] = sourceId;
            }

            if (!sourceDestinationDictionary.TryGetValue(destinationId, out _))
            {
                sourceDestinationDictionary[destinationId] = destinationId;
            }

            (sourceDestinationDictionary[destinationId], sourceDestinationDictionary[sourceId])
                = (sourceDestinationDictionary[sourceId], sourceDestinationDictionary[destinationId]);
        }

        foreach (var pairOfSourceDestination in sourceDestinationDictionary)
        {
            if (pairOfSourceDestination.Key == pairOfSourceDestination.Value)
            {
                sourceDestinationDictionary.Remove(pairOfSourceDestination.Key);
            }
        }

        var listOfIds = sourceDestinationDictionary.Select(x => x.Key).Distinct().ToList();
        var entities = existingEntities.Where(x => listOfIds.Contains(x.Id)).ToDictionary(x => x.Id);
        var entitiesClone = entities.ToDictionary(x => x.Key, x => x.Value.DisplayOrder);

        foreach (var pairOfSourceDestination in sourceDestinationDictionary)
        {
            entities[pairOfSourceDestination.Key].DisplayOrder = entitiesClone[pairOfSourceDestination.Value];
        }

        var entitiesToUpdate = entities.Values.ToList();
        var entitiesOrdered = await _entityRepository.UpdateRangeAsync(
            entitiesToUpdate,
            true,
            cancellationToken
        );

        _logger.LogInformation(
            "{BaseServiceName}.{UpdateAsyncName} - Successfully arrange ordered {EntityDataName}",
            nameof(BaseService<TEntity>),
            nameof(ArrangeSwapAsync),
            nameof(TEntity)
        );

        return entitiesOrdered;
    }

    public virtual async Task ArrangeOrderAsync(
        List<long> ids,
        CancellationToken cancellationToken = default
    )
    {
        var existingEntitiesCount = await GetQueryable()
            .Where(x => ids.Contains(x.Id))
            .CountAsync(cancellationToken);

        if (existingEntitiesCount != ids.Count)
        {
            throw _appNotfoundException;
        }

        await _entityRepository
            .GetQueryable()
            .Where(x => ids.Contains(x.Id))
            .ExecuteUpdateAsync(
                x =>
                    x.SetProperty(
                        t => t.DisplayOrder,
                        t => ids.Count - ids.IndexOf(t.Id)
                    ),
                cancellationToken
            );

        if (CacheService is not null)
        {
            await CacheService.ResetAsync($"{typeof(TEntity).Name}*", cancellationToken: cancellationToken);
        }
    }

    public virtual async Task<TEntity> DeleteAsync(
        long id,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var existingEntity = await FindByIdAsync(id, cancellationToken);
        existingEntity.IsDeleted = true;

        var entityRemove = await _entityRepository.UpdateAsync(
            existingEntity,
            autoSave,
            cancellationToken
        );

        _logger.LogInformation(
            "{BaseServiceName}.{DeleteAsyncName} - Successfully deleted {EntityDataName} {EntityRemoveId}",
            nameof(BaseService<TEntity>),
            nameof(DeleteAsync),
            nameof(TEntity),
            entityRemove.Id
        );

        return entityRemove;
    }

    public async Task<IEnumerable<TEntity>> DeleteRangeAsync(
        long[] ids,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var existingEntities = await GetQueryable()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
        if (existingEntities.Count != ids.Length)
        {
            throw _appNotfoundException;
        }

        foreach (var entity in existingEntities)
        {
            entity.IsDeleted = true;
        }

        var removeEntities = (await _entityRepository.UpdateRangeAsync(
            existingEntities,
            autoSave,
            cancellationToken
        )).ToList();

        _logger.LogInformation(
            "{BaseServiceName}.{DeleteAsyncName} - Successfully deleted {EntityDataName} {Count}",
            nameof(BaseService<TEntity>),
            nameof(DeleteRangeAsync),
            nameof(TEntity),
            removeEntities.Count
        );

        return removeEntities;
    }

    public async Task DeletePhysicalAsync(
        long id,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var existingEntity = await FindByIdAsync(id, cancellationToken);

        await _entityRepository.DeleteAsync(
            existingEntity,
            autoSave,
            cancellationToken
        );

        _logger.LogInformation(
            "{BaseServiceName}.{DeleteAsyncName} - Successfully deleted {EntityDataName} {EntityRemoveId}",
            nameof(BaseService<TEntity>),
            nameof(DeletePhysicalAsync),
            nameof(TEntity),
            existingEntity.Id
        );
    }

    public async Task DeletePhysicalRangeAsync(
        long[] ids,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var existingEntities = await GetQueryable()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
        if (existingEntities.Count != ids.Length)
        {
            throw _appNotfoundException;
        }

        await _entityRepository.DeleteRangeAsync(
            existingEntities,
            autoSave,
            cancellationToken
        );

        _logger.LogInformation(
            "{BaseServiceName}.{DeleteAsyncName} - Successfully deleted {EntityDataName} {Count}",
            nameof(BaseService<TEntity>),
            nameof(DeletePhysicalRangeAsync),
            nameof(TEntity),
            existingEntities.Count
        );
    }

    public virtual async Task<IPage<TEntity>> FindAllAsync(
        IPageable pageable,
        CancellationToken cancellationToken = default
    )
    {
        var key = JsonConvert.SerializeObject(
            pageable,
            JsonSettings.Optimized
        );

        var cacheKey = GetCacheKey(
            nameof(FindAllAsync),
            key
        );
        IPage<TEntity>? page = null;
        if (CacheService is not null)
        {
            page = await CacheService.GetAsync<Page<TEntity>>(
                cacheKey,
                cancellationToken
            );
        }

        if (page is null)
        {
            page = await GetQueryable()
                .OrderByDescending(x => x.DisplayOrder)
                .UsePageableAsync(
                    pageable,
                    cancellationToken: cancellationToken
                );

            if (CacheService is not null)
            {
                await CacheService.SetAsync(
                    cacheKey,
                    page,
                    cancellationToken
                );
            }
        }

        _logger.LogInformation(
            "{BaseServiceName}.{FindAllAsyncName} - Successfully found all {EntityDataName} {Count}",
            nameof(BaseService<TEntity>),
            nameof(FindAllAsync),
            nameof(TEntity),
            page.Content.Count()
        );

        return page;
    }

    public virtual async Task<IEnumerable<TEntity>> FindAllByIdsAsync(
        long[] ids,
        CancellationToken cancellationToken = default
    )
    {
        ids = [.. ids.OrderBy(x => x)];
        var cacheKey = GetCacheKey(
            nameof(FindAllByIdsAsync),
            string.Join('_', ids)
        );

        List<TEntity>? entities = null;
        if (CacheService is not null)
        {
            entities = await CacheService.GetAsync<List<TEntity>>(
                cacheKey,
                cancellationToken
            );
        }

        if (entities is null)
        {
            var queryable = GetQueryable().Where(x => ids.Contains(x.Id));
            entities = await queryable.ToListAsync(cancellationToken);

            if (CacheService is not null)
            {
                await CacheService.SetAsync(
                    cacheKey,
                    entities,
                    cancellationToken
                );
            }
        }

        _logger.LogInformation(
            "{BaseServiceName}.{FindByIdAsyncName} - Successfully found all {EntityDataName} {Count}",
            nameof(BaseService<TEntity>),
            nameof(FindAllByIdsAsync),
            nameof(TEntity),
            entities.Count
        );

        return entities;
    }

    public virtual async Task<TEntity> FindByIdAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        var cacheKey = GetCacheKey(
            nameof(FindByIdAsync),
            $"{id}"
        );
        TEntity? existingEntity = null;
        if (CacheService is not null)
        {
            existingEntity = await CacheService.GetAsync<TEntity>(
                cacheKey,
                cancellationToken
            );
        }

        if (existingEntity is null)
        {
            var queryable = GetQueryable();
            existingEntity = await queryable.SingleOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken
                )
                ?? throw _appNotfoundException;

            if (CacheService is not null)
            {
                await CacheService.SetAsync(
                    cacheKey,
                    existingEntity,
                    cancellationToken
                );
            }
        }

        _logger.LogInformation(
            "{BaseServiceName}.{FindByIdAsyncName} - Successfully found {EntityDataName} {EntityExistingId}",
            nameof(BaseService<TEntity>),
            nameof(FindByIdAsync),
            nameof(TEntity),
            existingEntity.Id
        );

        return existingEntity;
    }

    public virtual async Task<int> CountByIdsAsync(
        long[] ids,
        CancellationToken cancellationToken = default
    )
    {
        ids = [.. ids.OrderBy(x => x)];
        var cacheKey = GetCacheKey(
            nameof(CountByIdsAsync),
            string.Join('_', ids)
        );
        int? existingCount = null;
        if (CacheService is not null)
        {
            existingCount = await CacheService.GetAsync<int?>(
                cacheKey,
                cancellationToken
            );
        }

        if (existingCount is null)
        {
            var queryable = GetQueryable();
            existingCount = await queryable.CountAsync(
                x => ids.Contains(x.Id),
                cancellationToken
            );

            if (CacheService is not null)
            {
                await CacheService.SetAsync(
                    cacheKey,
                    existingCount,
                    cancellationToken
                );
            }
        }

        _logger.LogInformation(
            "{BaseServiceName}.{CountByIdsAsyncName} - Successfully found {EntityDataName} {Count}",
            nameof(BaseService<TEntity>),
            nameof(CountByIdsAsync),
            nameof(TEntity),
            existingCount
        );

        return existingCount.Value;
    }

    public virtual async Task<int> CountByCodesAsync(
        string[] codes,
        CancellationToken cancellationToken = default
    )
    {
        codes = [.. codes.OrderBy(x => x)];
        var cacheKey = GetCacheKey(
            nameof(CountByIdsAsync),
            string.Join('_', codes)
        );
        int? existingCount = null;
        if (CacheService is not null)
        {
            existingCount = await CacheService.GetAsync<int?>(
                cacheKey,
                cancellationToken
            );
        }

        if (existingCount is null)
        {
            var queryable = GetQueryable();
            existingCount = await queryable.CountAsync(
                x => codes.Contains(x.Code),
                cancellationToken
            );

            if (CacheService is not null)
            {
                await CacheService.SetAsync(
                    cacheKey,
                    existingCount,
                    cancellationToken
                );
            }
        }

        _logger.LogInformation(
            "{BaseServiceName}.{CountByCodesAsync} - Successfully found {EntityDataName} {Count}",
            nameof(BaseService<TEntity>),
            nameof(CountByCodesAsync),
            nameof(TEntity),
            existingCount
        );

        return existingCount.Value;
    }

    protected virtual IQueryable<TEntity> GetQueryable()
    {
        return _entityRepository
            .GetQueryableWithAsNoTracking();
    }
}
