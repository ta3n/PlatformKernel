namespace SharedKernel.Cache.Models;

/// <summary>
/// Represents the result of a single-flight cache retrieval operation.
/// </summary>
/// <typeparam name="T">The cached value type.</typeparam>
public sealed record SingleFlightCacheResult<T>(
    T? Value,
    SingleFlightCacheSource Source,
    TimeSpan Waited
);
