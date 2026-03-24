using Microsoft.Extensions.Logging;
using SharedKernel.Cache.Models;
using SharedKernel.Cache.Services;
using StackExchange.Redis;

namespace SharedKernel.Cache.Inventory;

/// <summary>
/// Provides atomic inventory reservation commands for booking flows that reserve stock across multiple dates.
/// </summary>
public sealed class RedisInventoryService(
    ILogger<RedisInventoryService> logger,
    IRedisCacheService redisCacheService
) : IRedisInventoryService
{
    private static readonly RedisInventoryOperationResult InvalidRequestResult = new()
    {
        Code = "INVALID_REQUEST",
        Status = "rejected"
    };

    public async Task SeedAvailabilityAsync(
        string inventoryId,
        IReadOnlyDictionary<DateOnly, long> dailyAvailability,
        int? database = null,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(inventoryId))
        {
            throw new ArgumentException("Inventory id is required.", nameof(inventoryId));
        }

        if (dailyAvailability.Count == 0)
        {
            return;
        }

        var pairs = dailyAvailability.ToDictionary(
            pair => RedisInventoryHelper.BuildAvailabilityKey(inventoryId, pair.Key),
            pair => Math.Max(0, pair.Value).ToString(),
            StringComparer.Ordinal
        );

        await redisCacheService.StringSetBulkAsync(
            pairs,
            new RedisCacheCommandOptions
            {
                Database = database
            },
            cancellationToken
        );
    }

    public async Task<IReadOnlyCollection<RedisInventoryAvailability>> GetAvailabilityAsync(
        string inventoryId,
        IEnumerable<DateOnly> dates,
        int? database = null,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(inventoryId))
        {
            throw new ArgumentException("Inventory id is required.", nameof(inventoryId));
        }

        var normalizedDates = RedisInventoryHelper.NormalizeDates(dates);

        if (normalizedDates.Count == 0)
        {
            return [];
        }

        var keyByDate = normalizedDates.ToDictionary(
            date => date,
            date => RedisInventoryHelper.BuildAvailabilityKey(inventoryId, date)
        );

        var values = await redisCacheService.StringGetManyAsync(
            keyByDate.Values,
            new RedisCacheCommandOptions
            {
                Database = database
            },
            cancellationToken
        );

        return normalizedDates
            .Select(
                date =>
                {
                    var rawValue = values.TryGetValue(keyByDate[date], out var value)
                        ? value
                        : null;

                    return new RedisInventoryAvailability(
                        date,
                        long.TryParse(rawValue, out var availableQuantity)
                            ? availableQuantity
                            : 0
                    );
                }
            )
            .ToArray();
    }

    public async Task<RedisInventoryOperationResult> ReserveAsync(
        RedisInventoryReservationRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (!IsValidReservationRequest(request))
        {
            return InvalidRequestResult with
            {
                ReservationId = request.ReservationId,
                InventoryId = request.InventoryId
            };
        }

        var normalizedDates = RedisInventoryHelper.NormalizeDates(request.Dates);
        var dataTtlSeconds = ResolveDataTtlSeconds(
            request.HoldTtl,
            request.ReservationDataTtl
        );
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var keys = normalizedDates
            .Select(date => RedisInventoryHelper.BuildAvailabilityKey(request.InventoryId, date))
            .Append(RedisInventoryHelper.BuildReservationKey(request.ReservationId))
            .Append(RedisInventoryHelper.BuildExpiringReservationsKey())
            .ToArray();
        var values = new List<RedisValue>
        {
            request.ReservationId,
            request.InventoryId,
            request.Quantity,
            Math.Max(1, (long)request.HoldTtl.TotalSeconds),
            dataTtlSeconds,
            now
        };

        values.AddRange(
            normalizedDates.Select(
                date => (RedisValue)RedisInventoryHelper.ToDateToken(date)
            )
        );

        var result = await redisCacheService.ExecuteLuaByNameWithResultAsync<RedisInventoryOperationResult>(
            "ReserveInventoryDateRange",
            keys,
            [.. values],
            new RedisCacheCommandOptions
            {
                Database = request.Database
            }
        );

        return result ?? InvalidRequestResult with
        {
            ReservationId = request.ReservationId,
            InventoryId = request.InventoryId
        };
    }

    public async Task<RedisInventoryOperationResult> ReleaseAsync(
        RedisInventoryReleaseRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(request.ReservationId))
        {
            return InvalidRequestResult;
        }

        var result = await redisCacheService.ExecuteLuaByNameWithResultAsync<RedisInventoryOperationResult>(
            "ReleaseInventoryReservation",
            [
                RedisInventoryHelper.BuildReservationKey(request.ReservationId),
                RedisInventoryHelper.BuildExpiringReservationsKey()
            ],
            [
                request.Reason,
                ResolveRetentionTtlSeconds(request.ReservationDataTtl),
                DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            ],
            new RedisCacheCommandOptions
            {
                Database = request.Database
            }
        );

        return result ?? InvalidRequestResult with
        {
            ReservationId = request.ReservationId
        };
    }

    public async Task<RedisInventoryOperationResult> ConfirmAsync(
        RedisInventoryConfirmRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(request.ReservationId))
        {
            return InvalidRequestResult;
        }

        var result = await redisCacheService.ExecuteLuaByNameWithResultAsync<RedisInventoryOperationResult>(
            "ConfirmInventoryReservation",
            [
                RedisInventoryHelper.BuildReservationKey(request.ReservationId),
                RedisInventoryHelper.BuildExpiringReservationsKey()
            ],
            [
                ResolveRetentionTtlSeconds(request.ReservationDataTtl),
                DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            ],
            new RedisCacheCommandOptions
            {
                Database = request.Database
            }
        );

        return result ?? InvalidRequestResult with
        {
            ReservationId = request.ReservationId
        };
    }

    public async Task<RedisInventoryReservationSnapshot?> GetReservationAsync(
        string reservationId,
        int? database = null,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(reservationId))
        {
            return null;
        }

        var values = await redisCacheService.HashGetAllAsync(
            RedisInventoryHelper.BuildReservationKey(reservationId),
            new RedisCacheCommandOptions
            {
                Database = database
            },
            cancellationToken
        );

        return values.Count == 0
            ? null
            : RedisInventoryReservationSnapshot.FromHash(values);
    }

    public async Task<IReadOnlyCollection<string>> GetExpiredReservationIdsAsync(
        DateTimeOffset asOfUtc,
        int take = 100,
        int? database = null,
        CancellationToken cancellationToken = default
    )
    {
        if (take <= 0)
        {
            return [];
        }

        var entries = await redisCacheService.GetDatabase(database)
            .SortedSetRangeByScoreAsync(
                redisCacheService.BuildKey(RedisInventoryHelper.BuildExpiringReservationsKey()),
                stop: asOfUtc.ToUnixTimeSeconds(),
                exclude: Exclude.None,
                order: Order.Ascending,
                take: take
            );

        return entries
            .Where(static entry => !entry.IsNullOrEmpty)
            .Select(static entry => entry.ToString())
            .ToArray();
    }

    public async Task<IReadOnlyCollection<RedisInventoryOperationResult>> ReleaseExpiredReservationsAsync(
        DateTimeOffset asOfUtc,
        int take = 100,
        TimeSpan? reservationDataTtl = null,
        int? database = null,
        CancellationToken cancellationToken = default
    )
    {
        var expiredReservationIds = await GetExpiredReservationIdsAsync(
            asOfUtc,
            take,
            database,
            cancellationToken
        );

        if (expiredReservationIds.Count == 0)
        {
            return [];
        }

        var results = new List<RedisInventoryOperationResult>(expiredReservationIds.Count);

        foreach (var reservationId in expiredReservationIds)
        {
            try
            {
                results.Add(
                    await ReleaseAsync(
                        new RedisInventoryReleaseRequest
                        {
                            ReservationId = reservationId,
                            Reason = "expired",
                            ReservationDataTtl = reservationDataTtl ?? TimeSpan.FromHours(12),
                            Database = database
                        },
                        cancellationToken
                    )
                );
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to release expired Redis inventory reservation {ReservationId}",
                    reservationId
                );
            }
        }

        return results;
    }

    private static bool IsValidReservationRequest(
        RedisInventoryReservationRequest request
    )
    {
        return !string.IsNullOrWhiteSpace(request.ReservationId)
               && !string.IsNullOrWhiteSpace(request.InventoryId)
               && request.Quantity > 0
               && request.Dates.Count > 0
               && request.HoldTtl > TimeSpan.Zero;
    }

    private static long ResolveDataTtlSeconds(
        TimeSpan holdTtl,
        TimeSpan reservationDataTtl
    )
    {
        var minimumRetention = holdTtl.Add(TimeSpan.FromHours(1));
        var effectiveTtl = reservationDataTtl > minimumRetention
            ? reservationDataTtl
            : minimumRetention;

        return Math.Max(1, (long)effectiveTtl.TotalSeconds);
    }

    private static long ResolveRetentionTtlSeconds(
        TimeSpan reservationDataTtl
    )
    {
        return Math.Max(1, (long)reservationDataTtl.TotalSeconds);
    }
}
