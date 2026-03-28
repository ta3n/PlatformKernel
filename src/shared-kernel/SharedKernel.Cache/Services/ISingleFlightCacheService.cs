using SharedKernel.Cache.Models;
using SharedKernel.Cache.Options;

namespace SharedKernel.Cache.Services;

/// <summary>
/// Coordinates distributed single-flight cache refresh operations to reduce cache stampedes.
/// </summary>
public interface ISingleFlightCacheService
{
    /// <summary>
    /// Gets a value from cache or creates it using a distributed lock so only one caller refreshes the cache at a time.
    /// </summary>
    /// <typeparam name="T">The cached value type.</typeparam>
    /// <param name="key">The primary cache key.</param>
    /// <param name="valueFactory">The origin factory used to populate the cache when needed.</param>
    /// <param name="options">Optional single-flight behavior overrides.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The single-flight cache result.</returns>
    Task<SingleFlightCacheResult<T>> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> valueFactory,
        SingleFlightCacheOptions? options = null,
        CancellationToken cancellationToken = default
    );
}
