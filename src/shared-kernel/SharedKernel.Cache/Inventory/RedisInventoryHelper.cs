using System.Globalization;

namespace SharedKernel.Cache.Inventory;

/// <summary>
/// Provides key building and date normalization helpers for Redis-based inventory reservation.
/// </summary>
public static class RedisInventoryHelper
{
    private const string InventoryAvailabilityPrefix = "inventory:availability";

    private const string InventoryReservationPrefix = "inventory:reservation";

    private const string InventoryExpiringReservationsKey = "inventory:reservation:expiring";

    /// <summary>
    /// Expands a stay window into booking dates using an exclusive check-out date.
    /// </summary>
    public static IReadOnlyList<DateOnly> ExpandStayDates(
        DateOnly checkIn,
        DateOnly checkOutExclusive
    )
    {
        if (checkOutExclusive <= checkIn)
        {
            throw new ArgumentException("Check-out date must be greater than check-in date.", nameof(checkOutExclusive));
        }

        var dates = new List<DateOnly>();

        for (var current = checkIn; current < checkOutExclusive; current = current.AddDays(1))
        {
            dates.Add(current);
        }

        return dates;
    }

    /// <summary>
    /// Normalizes booking dates into a distinct ascending list.
    /// </summary>
    public static IReadOnlyList<DateOnly> NormalizeDates(
        IEnumerable<DateOnly> dates
    )
    {
        return dates
            .Distinct()
            .OrderBy(static date => date)
            .ToArray();
    }

    /// <summary>
    /// Builds the availability key for a specific inventory item and date.
    /// </summary>
    public static string BuildAvailabilityKey(
        string inventoryId,
        DateOnly date
    )
    {
        return $"{InventoryAvailabilityPrefix}:{inventoryId}:{ToDateToken(date)}";
    }

    /// <summary>
    /// Builds the reservation metadata key.
    /// </summary>
    public static string BuildReservationKey(
        string reservationId
    )
    {
        return $"{InventoryReservationPrefix}:{reservationId}";
    }

    /// <summary>
    /// Gets the sorted-set key that tracks expiring reservations.
    /// </summary>
    public static string BuildExpiringReservationsKey()
    {
        return InventoryExpiringReservationsKey;
    }

    /// <summary>
    /// Converts a booking date to the compact token used inside Redis.
    /// </summary>
    public static string ToDateToken(
        DateOnly date
    )
    {
        return date.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Parses a comma-separated list of Redis date tokens.
    /// </summary>
    public static IReadOnlyList<DateOnly> ParseDateTokens(
        string? values
    )
    {
        if (string.IsNullOrWhiteSpace(values))
        {
            return [];
        }

        return values.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(
                value => DateOnly.ParseExact(
                    value,
                    "yyyyMMdd",
                    CultureInfo.InvariantCulture
                )
            )
            .ToArray();
    }
}
