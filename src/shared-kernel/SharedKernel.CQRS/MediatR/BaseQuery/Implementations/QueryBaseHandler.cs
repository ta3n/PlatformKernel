using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using SharedKernel.AppShared.Utils;
using SharedKernel.Cache.Services;
using SharedKernel.Entity.Utils;

namespace SharedKernel.CQRS.MediatR.BaseQuery.Implementations;

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
    /// Abstract base class for handling queries in a CQRS pattern.
    /// Provides shared functionality and common behaviors for query handlers, such as mapping, caching,
    /// and request validation.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query handled, which implements <see cref="IQueryBase{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
    protected QueryBaseHandler(
        ICacheService cacheService
    )
    {
        CacheService = cacheService;
    }

    protected QueryBaseHandler()
    {
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
            var cachedResponse = await TryGetFromCacheAsync(
                cacheKey,
                cancellationToken
            );
            if (cachedResponse.HasValue)
            {
                return cachedResponse.Value;
            }

            var response = await HandleAsync(
                request,
                cancellationToken
            );

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

                delay *= 2;
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
            var fallbackResponse = await HandleAsync(
                request,
                cancellationToken
            );

            return fallbackResponse;
        }

        try
        {
            var cachedResponse = await TryGetFromCacheAsync(
                cacheKey,
                cancellationToken
            );
            if (cachedResponse.HasValue)
            {
                return cachedResponse.Value;
            }

            var response = await HandleAsync(
                request,
                cancellationToken
            );

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

    private static readonly ConcurrentDictionary<string, Task<(IHeaderDictionary, TResponse)>> PendingTasks = new();

    private static readonly Func<string, (QueryBaseHandler<TQuery, TResponse> handler, TQuery request, string cacheKey,
            CancellationToken
            cancellationToken), Task<(IHeaderDictionary, TResponse)>>
        TaskFactoryDelegate = CreateTaskFactory;

    private async Task<(IHeaderDictionary, TResponse)> HandleWithTaskDeduplicationAsync(
        TQuery request,
        string cacheKey,
        CancellationToken cancellationToken
    )
    {
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
            return await HandleAsync(request, cancellationToken);
        }
        finally
        {
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

    private static Task<(IHeaderDictionary, TResponse)> CreateTaskFactory(
        string key,
        (QueryBaseHandler<TQuery, TResponse> handler, TQuery request, string cacheKey, CancellationToken
            cancellationToken) args
    )
    {
        return Task.Run(
            async () =>
            {
                var (handler, request, cacheKey, cancellationToken) = args;

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
    private static (IHeaderDictionary, T)? ConvertJsonToTuple<T>(
        string? json
    )
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        const string item1 = "item1";
        const string item2 = "item2";

        var jsonObject = JsonDocument.Parse(json).RootElement;

        var headers = new HeaderDictionary();
        foreach (var header in jsonObject.GetProperty(item1).EnumerateObject())
        {
            headers[header.Name] = new StringValues([.. header.Value.EnumerateArray().Select(x => x.GetString())]);
        }

        var data = JsonSerializer.Deserialize<T>(
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
        var requestJson = JsonSerializer.Serialize(
            request,
            JsonSettings.OptimizedSystemTextJson
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
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> CacheLocks = new();
    private static readonly ConcurrentDictionary<string, DateTime> LockTimestamps = new();
    private static readonly object CleanupLock = new();
    private static DateTime _lastCleanup = DateTime.UtcNow;

    private const int CleanupIntervalMinutes = 5;
    private const int LockExpirationMinutes = 10;

    public static SemaphoreSlim GetCacheLockForKey(
        string key
    )
    {
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
                    // Ignore disposal errors during best-effort cleanup.
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
    protected QuerySingleBaseHandler()
    {
    }

    protected QuerySingleBaseHandler(
        ICacheService cacheService
    ) : base(cacheService)
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
    protected QueryListBaseHandler()
    {
    }

    protected QueryListBaseHandler(
        ICacheService cacheService
    ) : base(cacheService)
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
    protected QueryPageBaseHandler()
    {
    }

    protected QueryPageBaseHandler(
        ICacheService cacheService
    ) : base(cacheService)
    {
    }
}
