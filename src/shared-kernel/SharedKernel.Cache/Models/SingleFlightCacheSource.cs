namespace SharedKernel.Cache.Models;

/// <summary>
/// Describes how a single-flight cache request was satisfied.
/// </summary>
public enum SingleFlightCacheSource
{
    CacheHit,
    WaitedCacheHit,
    Refreshed,
    StaleHit,
    OriginFallback,
    Bypassed
}
