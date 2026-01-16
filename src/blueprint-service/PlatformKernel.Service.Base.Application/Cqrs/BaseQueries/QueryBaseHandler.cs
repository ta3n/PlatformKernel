using System.Collections.Concurrent;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using PlatformKernel.ApplicationShared.Cqrs.BaseQuery;
using PlatformKernel.Cache.Services;
using PlatformKernel.Entity.Utils;
using PlatformKernel.Service.Base.Application.Constants;

namespace PlatformKernel.Service.Base.Application.Cqrs.BaseQueries;

/// Represents a base handler for processing queries in a CQRS (Command Query Responsibility Segregation) pattern.
/// This is an abstract class that provides foundational functionality for handling queries,
/// such as managing caching, request validation, and mapping.
/// Type Parameters:
/// - TQuery: The type of the query being handled. Must implement the IQueryBase&lt;TResponse&gt; interface.
/// - TResponse: The expected response type of the handled query.
public abstract class QueryBaseHandler<TQuery, TResponse>
    : IQueryBaseHandler<TQuery, TResponse> where TQuery : IQueryBase<TResponse>
{
    /// <summary>
    /// Provides access to the IMapper instance for mapping objects within the context of query handlers.
    /// </summary>
    /// <remarks>
    /// This property is utilized as a central mapping instance, enabling the transformation of entities and data transfer objects (DTOs) during query processing.
    /// </remarks>
    protected IMapper Mapper { get; }

    /// <summary>
    /// Provides caching capabilities for managing and retrieving data.
    /// This property allows interaction with a caching service to
    /// store and retrieve information with enhanced performance and lower latency.
    /// It is used to optimize operations by reducing redundant computation or data fetches.
    /// </summary>
    protected ICacheService? CacheService { get; }

    /// <summary>
    /// Strategy selector for concurrency handling.
    /// When true: uses DoubleCheckedLocking (better for high load)
    /// When false: uses TaskDeduplication (better for moderate load)
    /// </summary>
    private bool UseDoubleCheckedLockingStrategy { get; set; } = false;

    /// <summary>
    /// Specifies the number of seconds to wait for acquiring a cache lock before timing out.
    /// </summary>
    /// <remarks>
    /// This constant is utilized in scenarios where double-checked locking is implemented to
    /// prevent race conditions when accessing or modifying cache entries. The timeout defines
    /// the maximum duration to wait for the lock acquisition before a fallback mechanism is triggered.
    /// </remarks>
    private const int CacheLockTimeoutSeconds = 180;

    /// <summary>
    /// Serves as an abstract base class for handling query operations in a CQRS design pattern.
    /// Provides common functionality for handling queries, such as mapping and optional caching support.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query to be handled. Must implement <see cref="IQueryBase{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">The type of the response returned from the query execution.</typeparam>
    protected QueryBaseHandler(
        IMapper mapper
    )
    {
        Mapper = mapper;
    }

    /// <summary>
    /// Abstract base class for handling queries in a CQRS pattern.
    /// Provides shared functionality and common behaviors for query handlers, such as mapping, caching,
    /// and request validation.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query handled, which implements <see cref="IQueryBase{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
    protected QueryBaseHandler(
        IMapper mapper,
        ICacheService cacheService
    )
    {
        Mapper = mapper;
        CacheService = cacheService;
    }

    /// <summary>
    /// Validates the incoming request before processing it.
    /// </summary>
    /// <param name="request">The query request to validate.</param>
    /// <param name="cancellationToken">A cancellation token to observe during the validation process.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the request is valid.</returns>
    protected virtual Task<bool> IsValidRequest(
        TQuery request,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult(true);
    }

    /// <summary>
    /// Handles the incoming query by processing the request, optionally retrieving a cached response,
    /// and returning a tuple containing headers and a response object.
    /// </summary>
    /// <param name="request">The query object containing the details of the request.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A tuple where the first item is the response headers, and the second item is the response data.</returns>
    public virtual async Task<(IHeaderDictionary, TResponse)> Handle(
        TQuery request,
        CancellationToken cancellationToken
    )
    {
        var baseCacheKey = GetCacheKey(request) ?? string.Empty;
        var localizedCacheKey = BuildLocalizedCacheKey(baseCacheKey);
        var isUseCache = CacheService is not null || !string.IsNullOrEmpty(baseCacheKey);

        if (string.IsNullOrEmpty(baseCacheKey))
        {
            isUseCache = false;
        }

        if (!isUseCache)
        {
            var response = await HandleAsync(
                request,
                cancellationToken
            );
            return response;
        }

        var cachedResponse = await TryGetFromCacheAsync(
            localizedCacheKey,
            cancellationToken
        );
        if (cachedResponse.HasValue)
        {
            return cachedResponse.Value;
        }

        if (CacheService is not null || !string.IsNullOrEmpty(baseCacheKey))
        {
            return await HandleWithCacheDistributedLockAsync(
                request,
                localizedCacheKey,
                cancellationToken
            );
        }

        if (UseDoubleCheckedLockingStrategy)
        {
            return await HandleWithDoubleCheckedLockingAsync(
                request,
                localizedCacheKey,
                cancellationToken
            );
        }

        return await HandleWithTaskDeduplicationAsync(
            request,
            localizedCacheKey,
            cancellationToken
        );

        static string BuildLocalizedCacheKey(
            string cacheKey
        )
        {
            var languageCode = LanguageHeaderUtil.GetLanguageCodeFromHeader();
            return $"{cacheKey}.{languageCode}";
        }
    }

    /// <summary>
    /// Attempts to retrieve a cached response from the cache service using the specified cache key.
    /// </summary>
    /// <param name="cacheKey">The key used to identify the cached item in the cache service.</param>
    /// <param name="cancellationToken">A token that can be used to propagate notification that the operation should be canceled.</param>
    /// <returns>
    /// A tuple containing the headers and the response value retrieved from the cache,
    /// or null if no value is found in the cache.
    /// </returns>
    private async Task<(IHeaderDictionary, TResponse)?> TryGetFromCacheAsync(
        string cacheKey,
        CancellationToken cancellationToken
    )
    {
        if (CacheService is null || string.IsNullOrEmpty(cacheKey))
        {
            return null;
        }

        var cacheJson = await CacheService!.GetStringAsync(cacheKey, cancellationToken);

        return string.IsNullOrEmpty(cacheJson) ? null : ConvertJsonToTuple<TResponse>(cacheJson);
    }

    /// <summary>
    /// Handles the query operation with a distributed caching lock mechanism to ensure
    /// that cache access and modifications are performed in a thread-safe manner.
    /// </summary>
    /// <param name="request">The query request object containing the necessary details for query processing.</param>
    /// <param name="cacheKey">The unique identifier for the cache entry related to the query request.</param>
    /// <param name="cancellationToken">
    /// A token to observe for cancellation requests, enabling cooperative cancellation of the operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a tuple
    /// containing the response headers and the processed query response data.
    /// </returns>
    /// <remarks>
    /// This method leverages a distributed lock provided by the <see cref="ICacheService"/> to ensure that only one process
    /// can modify the cache for a specific key concurrently. A fallback mechanism is implemented to execute the query
    /// directly if the lock cannot be acquired. If the data is not already cached, it handles query execution and stores
    /// the result in the cache for future requests.
    /// </remarks>
    private async Task<(IHeaderDictionary, TResponse)> HandleWithCacheDistributedLockAsync(
        TQuery request,
        string cacheKey,
        CancellationToken cancellationToken
    )
    {
        var lockKey = $"lock:{cacheKey}";
        var lockId = Guid.NewGuid().ToString();
        var lockTimeout = TimeSpan.FromSeconds(CacheLockTimeoutSeconds);

        var acquired = await RetryWithExponentialBackoffAsync(
            () => CacheService!.AcquireLockAsync(lockKey, lockId, lockTimeout),
            4,
            1000,
            cancellationToken
        );
        if (!acquired)
        {
            var cachedBeforeFallback = await TryGetFromCacheAsync(cacheKey, cancellationToken);
            if (cachedBeforeFallback.HasValue)
            {
                return cachedBeforeFallback.Value;
            }

            var fallbackResponse = await HandleAsync(
                request,
                cancellationToken
            );

            await CacheService!.SetAsync(
                cacheKey,
                fallbackResponse,
                cancellationToken
            );

            return fallbackResponse;
        }

        try
        {
            // Double-check if data is now available in cache
            var cachedResponse = await TryGetFromCacheAsync(
                cacheKey,
                cancellationToken
            );
            if (cachedResponse.HasValue)
            {
                return cachedResponse.Value;
            }

            // Execute and cache the result
            var response = await HandleAsync(
                request,
                cancellationToken
            );

            // Set cache with expiration (you should implement this in your ICacheService)
            await CacheService!.SetAsync(
                cacheKey,
                response,
                cancellationToken
            );

            return response;
        }
        finally
        {
            await CacheService!.ReleaseLockAsync(lockKey, lockId);
        }

        static async Task<bool> RetryWithExponentialBackoffAsync(
            Func<Task<bool>> tryLockFunc,
            int maxAttempts,
            int initialDelayMs,
            CancellationToken token
        )
        {
            var delay = initialDelayMs;

            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                token.ThrowIfCancellationRequested();

                var lockAcquisitionResult = await tryLockFunc();
                if (lockAcquisitionResult)
                {
                    return true;
                }

                await Task.Delay(delay, token);

                delay *= 2; // exponential
                delay += Random.Shared.Next(0, 50);
            }

            return false;
        }
    }

    /// <summary>
    /// Executes the provided query with double-checked locking to ensure thread safety during concurrent cache reads and writes.
    /// If the response is not found in the cache, it is retrieved by executing the query handler
    /// and subsequently stored in the cache before being returned.
    /// </summary>
    /// <param name="request">The query request to be processed.</param>
    /// <param name="cacheKey">The cache key used to store or retrieve the response.</param>
    /// <param name="cancellationToken">A token for monitoring cancellation requests.</param>
    /// <returns>
    /// A tuple consisting of response headers and the response data retrieved either from the cache or by executing the query.
    /// </returns>
    private async Task<(IHeaderDictionary, TResponse)> HandleWithDoubleCheckedLockingAsync(
        TQuery request,
        string cacheKey,
        CancellationToken cancellationToken
    )
    {
        var lockObj = QueryCacheLockManager.GetCacheLockForKey(cacheKey);
        var acquired = await lockObj.WaitAsync(
            TimeSpan.FromSeconds(CacheLockTimeoutSeconds),
            cancellationToken
        );

        if (!acquired)
        {
            // Fallback: execute without caching
            var fallbackResponse = await HandleAsync(
                request,
                cancellationToken
            );

            return fallbackResponse;
        }

        try
        {
            // Double-check if data is now available in cache
            var cachedResponse = await TryGetFromCacheAsync(
                cacheKey,
                cancellationToken
            );
            if (cachedResponse.HasValue)
            {
                return cachedResponse.Value;
            }

            // Execute and cache the result
            var response = await HandleAsync(
                request,
                cancellationToken
            );

            // Set cache with expiration (you should implement this in your ICacheService)
            if (CacheService is null)
            {
                return response;
            }

            await CacheService!.SetAsync(
                cacheKey,
                response,
                cancellationToken
            );

            return response;
        }
        finally
        {
            lockObj.Release();
        }
    }

    /// <summary>
    /// Represents a thread-safe collection of currently pending query handling tasks for deduplication purposes.
    /// </summary>
    /// <remarks>
    /// This concurrent dictionary is utilized to manage asynchronous tasks that are in progress,
    /// ensuring that multiple identical query requests with the same cache key are deduplicated and
    /// resolved via a single task instance. It helps in reducing redundant processing and improving system efficiency.
    /// </remarks>
    private static readonly ConcurrentDictionary<string, Task<(IHeaderDictionary, TResponse)>> PendingTasks = new();

    /// <summary>
    /// Represents a static delegate function used to create and manage tasks for handling operations with task deduplication.
    /// </summary>
    /// <remarks>
    /// This delegate is employed to encapsulate the logic for generating asynchronous operations tied to specific handlers
    /// and requests. It is primarily used in conjunction with caching mechanisms to ensure that duplicate tasks for the same
    /// key or operation are not redundantly executed, thereby increasing efficiency and performance.
    /// </remarks>
    private static readonly Func<string, (QueryBaseHandler<TQuery, TResponse> handler, TQuery request, string cacheKey, CancellationToken
            cancellationToken), Task<(IHeaderDictionary, TResponse)>>
        TaskFactoryDelegate = CreateTaskFactory;

    /// <summary>
    /// Executes the query handling logic with task deduplication to avoid processing duplicate queries simultaneously.
    /// Ensures that repeated requests with the same cache key share the same ongoing task, improving efficiency and preventing redundant processing.
    /// </summary>
    /// <param name="request">The query instance to be handled.</param>
    /// <param name="cacheKey">The unique cache key associated with the request for deduplication purposes.</param>
    /// <param name="cancellationToken">A token for propagating cancellation notifications.</param>
    /// <returns>A task that represents the asynchronous operation. The result contains a tuple with headers and the query response.</returns>
    private async Task<(IHeaderDictionary, TResponse)> HandleWithTaskDeduplicationAsync(
        TQuery request,
        string cacheKey,
        CancellationToken cancellationToken
    )
    {
        // Get or add task - using static delegate
        var task = PendingTasks.GetOrAdd(
            cacheKey,
            TaskFactoryDelegate,
            (this, request, cacheKey, cancellationToken)
        );

        try
        {
            var result = await task.WaitAsync(
                TimeSpan.FromSeconds(CacheLockTimeoutSeconds),
                cancellationToken
            );

            return result;
        }
        catch (TimeoutException)
        {
            // Fallback execution
            return await HandleAsync(request, cancellationToken);
        }
        finally
        {
            // Cleanup completed task after a delay to allow other requests to benefit
            _ = Task
                .Delay(TimeSpan.FromSeconds(5), CancellationToken.None)
                .ContinueWith(
                    _ =>
                    {
                        PendingTasks.TryRemove(cacheKey, out var _);
                        return Task.CompletedTask;
                    },
                    cancellationToken
                );
        }
    }

    // Static factory method để tránh allocation
    /// <summary>
    /// Creates and returns a task responsible for handling the query execution and optional caching logic in a CQRS pattern.
    /// Used as a static factory method to minimize memory allocations during task creation for query handling.
    /// </summary>
    /// <param name="key">A unique string identifier used for caching and task correlation.</param>
    /// <param name="args">A tuple containing the query handler, the query request, the cache key, and a cancellation token.</param>
    /// <returns>
    /// A task that contains the tuple of response headers and the query response, wrapped in a caching mechanism where applicable.
    /// </returns>
    private static Task<(IHeaderDictionary, TResponse)> CreateTaskFactory(
        string key,
        (QueryBaseHandler<TQuery, TResponse> handler, TQuery request, string cacheKey, CancellationToken cancellationToken) args
    )
    {
        return Task.Run(
            async () =>
            {
                var (handler, request, cacheKey, cancellationToken) = args;

                // Double-check cache before execution
                var cachedResponse = await handler.TryGetFromCacheAsync(
                    cacheKey,
                    cancellationToken
                );
                if (cachedResponse.HasValue)
                {
                    return cachedResponse.Value;
                }

                var response = await handler.HandleAsync(
                    request,
                    cancellationToken
                );

                // Cache result
                await handler.CacheService!.SetAsync(
                    cacheKey,
                    response,
                    cancellationToken
                );

                return response;
            }
        );
    }

    /// <summary>
    /// Handles the provided query asynchronously and processes it to generate a response.
    /// </summary>
    /// <param name="request">The query request to be handled.</param>
    /// <param name="cancellationToken">Token used to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with the response headers and the actual processed response.</returns>
    protected abstract Task<(IHeaderDictionary, TResponse)> HandleAsync(
        TQuery request,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Generates a cache key based on the provided request object.
    /// This method can be overridden in derived classes to provide specific implementations
    /// of generating a unique cache key for the query.
    /// </summary>
    /// <param name="request">The query request object for which the cache key is generated.</param>
    /// <returns>A string representation of the cache key if implemented, otherwise null.</returns>
    protected virtual string? GetCacheKey(
        TQuery request
    )
    {
        return null;
    }

    /// <summary>
    /// Converts a JSON string into a tuple containing headers and data of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the data object to be deserialized from the JSON.</typeparam>
    /// <param name="json">The JSON string to be converted into a tuple. Should contain properties "item1" and "item2".</param>
    /// <return>
    /// A tuple containing headers represented by <see cref="IHeaderDictionary"/> and the deserialized data of type <typeparamref name="T"/>.
    /// Returns null if the provided JSON string is null or empty.
    /// </return>
    protected static (IHeaderDictionary, T)? ConvertJsonToTuple<T>(
        string? json
    )
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        const string item1 = "item1";
        const string item2 = "item2";

        var jsonObject = System.Text.Json.JsonDocument.Parse(json).RootElement;

        var headers = new HeaderDictionary();
        foreach (var header in jsonObject.GetProperty(item1).EnumerateObject())
        {
            headers[header.Name] = new StringValues([.. header.Value.EnumerateArray().Select(x => x.GetString())]);
        }

        var data = JsonConvert.DeserializeObject<T>(
            jsonObject.GetProperty(item2).GetRawText()
        );

        return (headers, data!);
    }

    /// <summary>
    /// Serializes the given query request into a JSON string representation.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query being processed.</typeparam>
    /// <typeparam name="TResponse">The type of the response expected from the query.</typeparam>
    /// <param name="request">The query request instance to be serialized.</param>
    /// <returns>A JSON string representing the serialized query request.</returns>
    /// <remarks>
    /// This method utilizes JSON serialization optimized for the application-specific settings.
    /// It is commonly used to generate cache keys or log serialized request data.
    /// </remarks>
    protected static string GetRequestJson(
        TQuery request
    )
    {
        var requestJson = JsonConvert.SerializeObject(
            request,
            JsonSettings.Optimized
        );

        return requestJson;
    }
}

/// Provides a mechanism to manage and synchronize access to cache data using locks.
/// It ensures thread-safe operations by utilizing a `SemaphoreSlim` for each unique cache key.
/// This class is typically used in scenarios where multiple threads or requests
/// might try to access or modify cached data for the same key concurrently.
public static class QueryCacheLockManager
{
    /// <summary>
    /// Maintains a thread-safe collection of semaphore locks associated with specific cache keys.
    /// </summary>
    /// <remarks>
    /// This static dictionary is used to ensure serialized access to cache resources by associating a
    /// SemaphoreSlim instance with a specific cache key. This prevents concurrent operations
    /// from modifying or accessing the same cache entry at the same time, enabling thread safety
    /// for cache operations.
    /// </remarks>
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> CacheLocks = new();

    /// <summary>
    /// Stores timestamps associated with active cache locks, mapping unique cache keys to their last access or update time.
    /// </summary>
    /// <remarks>
    /// This dictionary is used to monitor and manage the lifecycle of active cache locks in the system.
    /// It helps facilitate periodic cleanup of expired locks, ensuring optimal memory usage and preventing stale lock retention.
    /// Entries in this collection are updated whenever a cache key is accessed or its associated lock is acquired.
    /// </remarks>
    private static readonly ConcurrentDictionary<string, DateTime> LockTimestamps = new();

    /// <summary>
    /// Serves as a synchronization primitive to ensure thread-safe access during periodic cache cleanup operations.
    /// </summary>
    /// <remarks>
    /// This static lock is utilized within the cache cleanup mechanism to prevent race conditions and ensure
    /// that only one thread can perform the cleanup process at a time, thus maintaining data consistency and minimizing resource contention.
    /// </remarks>
    private static readonly object CleanupLock = new();

    /// <summary>
    /// Tracks the timestamp of the last cleanup operation within the cache lock manager.
    /// </summary>
    /// <remarks>
    /// Used to determine if the periodic cleanup of expired locks needs to be executed,
    /// based on the elapsed time since the last cleanup. Helps maintain efficient memory
    /// usage by removing expired locks at regular intervals.
    /// </remarks>
    private static DateTime _lastCleanup = DateTime.UtcNow;

    /// <summary>
    /// Specifies the interval, in minutes, for performing cleanup of expired cache locks.
    /// </summary>
    /// <remarks>
    /// This constant determines how often the system checks for and removes expired locks in the cache
    /// to ensure optimal memory usage and to prevent stale data from persisting in the lock manager.
    /// </remarks>
    private const int CleanupIntervalMinutes = 5;

    /// <summary>
    /// Specifies the duration, in minutes, after which an unused lock expires and is eligible for removal during cleanup.
    /// </summary>
    /// <remarks>
    /// This value determines the time threshold for cleaning up expired locks that are no longer associated with ongoing operations.
    /// Locks older than this duration are considered stale and are cleared to free up resources and maintain optimal performance.
    /// </remarks>
    private const int LockExpirationMinutes = 10;

    /// <summary>
    /// Retrieves or creates a <see cref="SemaphoreSlim"/> instance associated with a specific cache key.
    /// This provides a mechanism to synchronize access to resources or operations tied to the provided key.
    /// </summary>
    /// <param name="key">The unique key for which to retrieve or create the lock.</param>
    /// <returns>A <see cref="SemaphoreSlim"/> instance used to coordinate access for the specified key.</returns>
    public static SemaphoreSlim GetCacheLockForKey(
        string key
    )
    {
        // return CacheLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

        // Periodic cleanup to prevent memory leaks
        if (DateTime.UtcNow.Subtract(_lastCleanup).TotalMinutes > CleanupIntervalMinutes)
        {
            lock (CleanupLock)
            {
                if (DateTime.UtcNow.Subtract(_lastCleanup).TotalMinutes > CleanupIntervalMinutes)
                {
                    CleanupExpiredLocks();
                    _lastCleanup = DateTime.UtcNow;
                }
            }
        }

        var semaphore = CacheLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 100));

        // Track usage timestamp for cleanup
        LockTimestamps.AddOrUpdate(
            key,
            DateTime.UtcNow,
            (
                _,
                _
            ) => DateTime.UtcNow
        );

        return semaphore;
    }

    /// <summary>
    /// Cleans up expired locks to prevent memory leaks
    /// </summary>
    private static void CleanupExpiredLocks()
    {
        var cutoffTime = DateTime.UtcNow.AddMinutes(-LockExpirationMinutes);

        var keysToRemove = (from kvp in LockTimestamps where kvp.Value < cutoffTime select kvp.Key).ToList();

        foreach (var key in keysToRemove)
        {
            if (CacheLocks.TryRemove(key, out var semaphore))
            {
                try
                {
                    semaphore.Dispose();
                }
                catch
                {
                    // Ignore disposal errors
                }
            }

            LockTimestamps.TryRemove(key, out _);
        }
    }
}

/// <summary>
/// An abstract base handler for executing single query operations.
/// This handler provides a foundation for implementing query logic
/// that involves returning a single response.
/// </summary>
/// <typeparam name="TQuery">
/// The query type, inheriting from <see cref="IQuerySingleBase{TResponse}"/> and
/// <see cref="IQueryBase{TResponse}"/>. Represents the input data required to execute the query.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned after processing the query.
/// </typeparam>
public abstract class QuerySingleBaseHandler<TQuery, TResponse>
    : QueryBaseHandler<TQuery, TResponse>, IQuerySingBaseHandler<TQuery, TResponse>
    where TQuery : IQuerySingleBase<TResponse>, IQueryBase<TResponse>
{
    /// <summary>
    /// Represents an abstract handler specifically designed to handle single-query operations.
    /// This class extends the functionality provided by QueryBaseHandler and enforces the
    /// handling of queries implementing <see cref="IQuerySingleBase{TResponse}"/>.
    /// </summary>
    /// <typeparam name="TQuery">
    /// The type of query being handled. It must implement <see cref="IQuerySingleBase{TResponse}"/>
    /// and <see cref="IQueryBase{TResponse}"/>.
    /// </typeparam>
    /// <typeparam name="TResponse">
    /// The type of response returned after the query is processed.
    /// </typeparam>
    protected QuerySingleBaseHandler(
        IMapper mapper
    ) : base(mapper)
    {
    }

    /// <summary>
    /// Represents a base handler for processing single query requests within the application's CQRS framework.
    /// Inherits from <see cref="QueryBaseHandler{TQuery, TResponse}"/> and implements <see cref="IQuerySingBaseHandler{TQuery, TResponse}"/>.
    /// It is designed to handle single, specific types of queries that produce a single response.
    /// </summary>
    /// <typeparam name="TQuery">
    /// The type of the query being processed. Must implement <see cref="IQuerySingleBase{TResponse}"/> and <see cref="IQueryBase{TResponse}"/>.
    /// </typeparam>
    /// <typeparam name="TResponse">
    /// The type of the response that the handler produces.
    /// </typeparam>
    protected QuerySingleBaseHandler(
        IMapper mapper,
        ICacheService cacheService
    ) : base(mapper, cacheService)
    {
    }
}

/// <summary>
/// Represents a base class for handling list-based query requests, providing functionality for
/// processing and responding to queries that return collections of generic response objects.
/// </summary>
/// <typeparam name="TQuery">
/// The type of the query being handled, implementing the <see cref="IQueryListBase{TResponse}"/> interface.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response objects contained within the output collection.
/// </typeparam>
public abstract class QueryListBaseHandler<TQuery, TResponse>
    : QueryBaseHandler<TQuery, IEnumerable<TResponse>>, IQueryListBaseHandler<TQuery, TResponse>
    where TQuery : IQueryListBase<TResponse>
{
    /// <summary>
    /// Represents an abstract base class for handling queries that return a list of responses.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query being processed. Must implement <see cref="IQueryListBase{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">The type of the response returned as a list.</typeparam>
    /// <remarks>
    /// This handler class is intended to provide a foundation for handling queries that result in a collection of items.
    /// It builds upon the <see cref="QueryBaseHandler{TQuery, TResponse}"/> class, allowing additional functionality to be added as needed.
    /// </remarks>
    protected QueryListBaseHandler(
        IMapper mapper
    ) : base(mapper)
    {
    }

    /// <summary>
    /// Represents a base handler for managing list-based query operations within the CQRS pattern.
    /// This abstract class extends the functionality of <see cref="QueryBaseHandler{TQuery, TResponse}"/>
    /// to handle queries that return a collection of responses.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query object that implements <see cref="IQueryListBase{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">The type of the response elements returned by the query.</typeparam>
    protected QueryListBaseHandler(
        IMapper mapper,
        ICacheService cacheService
    ) : base(mapper, cacheService)
    {
    }
}

/// <summary>
/// Represents a base handler for paged query operations in the application.
/// This class serves as an abstraction for handling queries that involve pagination and processes a collection of results.
/// </summary>
/// <typeparam name="TQuery">The type of the query being handled. This must implement <see cref="IQueryPagedBase{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the query.</typeparam>
public abstract class QueryPageBaseHandler<TQuery, TResponse>
    : QueryBaseHandler<TQuery, IEnumerable<TResponse>>, IQueryPagedBaseHandler<TQuery, TResponse>
    where TQuery : IQueryPagedBase<TResponse>
{
    /// <summary>
    /// Represents a base handler for handling paged queries, providing common functionality
    /// for processing queries that return a paginated collection of response items.
    /// </summary>
    /// <typeparam name="TQuery">
    /// The type of the query being handled. Must implement <see cref="IQueryPagedBase{TResponse}"/>.
    /// </typeparam>
    /// <typeparam name="TResponse">
    /// The type of the response returned by the query handler. Represents individual items in the paginated collection.
    /// </typeparam>
    protected QueryPageBaseHandler(
        IMapper mapper
    ) : base(mapper)
    {
    }

    /// <summary>
    /// Serves as an abstract base class for handling paginated query operations in a CQRS design pattern.
    /// Extends the functionality of <see cref="QueryBaseHandler{TQuery, IEnumerable}"/>
    /// while additionally implementing support for paginated query handling through <see cref="IQueryPagedBaseHandler{TQuery, TResponse}"/>.
    /// Provides common functionality for managing queries, mapping, and caching in scenarios requiring pagination.
    /// </summary>
    /// <typeparam name="TQuery">
    /// The type of the paginated query to be handled, which must implement <see cref="IQueryPagedBase{TResponse}"/>.
    /// </typeparam>
    /// <typeparam name="TResponse">
    /// The type of the individual response item returned by the query handler.
    /// </typeparam>
    protected QueryPageBaseHandler(
        IMapper mapper,
        ICacheService cacheService
    ) : base(mapper, cacheService)
    {
    }
}
