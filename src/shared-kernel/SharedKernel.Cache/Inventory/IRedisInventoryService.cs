namespace SharedKernel.Cache.Inventory;

/// <summary>
/// Provides Redis-backed inventory reservation primitives for high-concurrency booking flows.
/// </summary>
public interface IRedisInventoryService
{
    Task SeedAvailabilityAsync(
        string inventoryId,
        IReadOnlyDictionary<DateOnly, long> dailyAvailability,
        int? database = null,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyCollection<RedisInventoryAvailability>> GetAvailabilityAsync(
        string inventoryId,
        IEnumerable<DateOnly> dates,
        int? database = null,
        CancellationToken cancellationToken = default
    );

    Task<RedisInventoryOperationResult> ReserveAsync(
        RedisInventoryReservationRequest request,
        CancellationToken cancellationToken = default
    );

    Task<RedisInventoryOperationResult> ReleaseAsync(
        RedisInventoryReleaseRequest request,
        CancellationToken cancellationToken = default
    );

    Task<RedisInventoryOperationResult> ConfirmAsync(
        RedisInventoryConfirmRequest request,
        CancellationToken cancellationToken = default
    );

    Task<RedisInventoryReservationSnapshot?> GetReservationAsync(
        string reservationId,
        int? database = null,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyCollection<string>> GetExpiredReservationIdsAsync(
        DateTimeOffset asOfUtc,
        int take = 100,
        int? database = null,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyCollection<RedisInventoryOperationResult>> ReleaseExpiredReservationsAsync(
        DateTimeOffset asOfUtc,
        int take = 100,
        TimeSpan? reservationDataTtl = null,
        int? database = null,
        CancellationToken cancellationToken = default
    );
}
