using System.Globalization;

namespace SharedKernel.Cache.Inventory;

/// <summary>
/// Represents the availability for an inventory item on a single day.
/// </summary>
public sealed record RedisInventoryAvailability(
    DateOnly Date,
    long AvailableQuantity
);

/// <summary>
/// Represents a reservation request for inventory held across multiple days.
/// </summary>
public sealed record RedisInventoryReservationRequest
{
    public required string ReservationId { get; init; }

    public required string InventoryId { get; init; }

    public required IReadOnlyCollection<DateOnly> Dates { get; init; }

    public long Quantity { get; init; } = 1;

    public TimeSpan HoldTtl { get; init; } = TimeSpan.FromMinutes(10);

    public TimeSpan ReservationDataTtl { get; init; } = TimeSpan.FromHours(12);

    public int? Database { get; init; }
}

/// <summary>
/// Represents a release request for an existing inventory reservation.
/// </summary>
public sealed record RedisInventoryReleaseRequest
{
    public required string ReservationId { get; init; }

    public string Reason { get; init; } = "released";

    public TimeSpan ReservationDataTtl { get; init; } = TimeSpan.FromHours(12);

    public int? Database { get; init; }
}

/// <summary>
/// Represents a confirmation request for an existing inventory reservation.
/// </summary>
public sealed record RedisInventoryConfirmRequest
{
    public required string ReservationId { get; init; }

    public TimeSpan ReservationDataTtl { get; init; } = TimeSpan.FromHours(12);

    public int? Database { get; init; }
}

/// <summary>
/// Represents the result of a Redis inventory reservation command.
/// </summary>
public sealed record RedisInventoryOperationResult
{
    public string Code { get; init; } = string.Empty;

    public string? Status { get; init; }

    public string? ReservationId { get; init; }

    public string? InventoryId { get; init; }

    public string? FailedDate { get; init; }

    public long? Available { get; init; }

    public long? ExpiresAtUnix { get; init; }

    public DateTimeOffset? ExpiresAtUtc => ExpiresAtUnix.HasValue
        ? DateTimeOffset.FromUnixTimeSeconds(ExpiresAtUnix.Value)
        : null;

    public bool Success => Code is "RESERVED" or "RELEASED" or "CONFIRMED" or "EXISTS";
}

/// <summary>
/// Represents the stored reservation metadata used for compensation and recovery.
/// </summary>
public sealed record RedisInventoryReservationSnapshot
{
    public required string ReservationId { get; init; }

    public required string InventoryId { get; init; }

    public required string Status { get; init; }

    public required long Quantity { get; init; }

    public required IReadOnlyList<DateOnly> Dates { get; init; }

    public DateTimeOffset? CreatedAtUtc { get; init; }

    public DateTimeOffset? ExpiresAtUtc { get; init; }

    public DateTimeOffset? ConfirmedAtUtc { get; init; }

    public DateTimeOffset? ReleasedAtUtc { get; init; }

    internal static RedisInventoryReservationSnapshot? FromHash(
        IReadOnlyDictionary<string, string?> values
    )
    {
        if (!values.TryGetValue("reservationId", out var reservationId) || string.IsNullOrWhiteSpace(reservationId))
        {
            return null;
        }

        if (!values.TryGetValue("inventoryId", out var inventoryId) || string.IsNullOrWhiteSpace(inventoryId))
        {
            return null;
        }

        if (!values.TryGetValue("status", out var status) || string.IsNullOrWhiteSpace(status))
        {
            return null;
        }

        if (!values.TryGetValue("quantity", out var quantityValue) || !long.TryParse(quantityValue, out var quantity))
        {
            return null;
        }

        var dates = values.TryGetValue("dates", out var dateValues)
            ? RedisInventoryHelper.ParseDateTokens(dateValues)
            : [];

        return new RedisInventoryReservationSnapshot
        {
            ReservationId = reservationId,
            InventoryId = inventoryId,
            Status = status,
            Quantity = quantity,
            Dates = dates,
            CreatedAtUtc = ParseUnixTimestamp(values, "createdAtUnix"),
            ExpiresAtUtc = ParseUnixTimestamp(values, "expiresAtUnix"),
            ConfirmedAtUtc = ParseUnixTimestamp(values, "confirmedAtUnix"),
            ReleasedAtUtc = ParseUnixTimestamp(values, "releasedAtUnix")
        };
    }

    private static DateTimeOffset? ParseUnixTimestamp(
        IReadOnlyDictionary<string, string?> values,
        string key
    )
    {
        return values.TryGetValue(key, out var rawValue)
               && long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var timestamp)
            ? DateTimeOffset.FromUnixTimeSeconds(timestamp)
            : null;
    }
}
