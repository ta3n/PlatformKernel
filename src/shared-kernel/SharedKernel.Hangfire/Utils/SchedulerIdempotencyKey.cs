using System.Globalization;

namespace SharedKernel.Hangfire.Utils;

/// <summary>
/// Builds stable idempotency keys for scheduler job occurrences.
/// </summary>
public static class SchedulerIdempotencyKey
{
    /// <summary>
    /// Creates an idempotency key from a job key and scheduled timestamp.
    /// </summary>
    /// <param name="jobKey">The stable scheduler job key.</param>
    /// <param name="scheduledAt">The due timestamp.</param>
    /// <returns>The idempotency key.</returns>
    public static string Create(
        string jobKey,
        DateTimeOffset scheduledAt
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jobKey);

        return string.Create(
            CultureInfo.InvariantCulture,
            $"{jobKey}:{scheduledAt.ToUniversalTime():O}"
        );
    }
}
