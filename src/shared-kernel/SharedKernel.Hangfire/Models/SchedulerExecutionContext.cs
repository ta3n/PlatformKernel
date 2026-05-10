using SharedKernel.Hangfire.Utils;

namespace SharedKernel.Hangfire.Models;

/// <summary>
/// Represents stable execution metadata for one scheduler job occurrence.
/// </summary>
public sealed record SchedulerExecutionContext
{
    /// <summary>
    /// Gets the stable scheduler job key.
    /// </summary>
    public required string JobKey { get; init; }

    /// <summary>
    /// Gets the due timestamp used to compute the idempotency key.
    /// </summary>
    public required DateTimeOffset ScheduledAt { get; init; }

    /// <summary>
    /// Gets the unique execution id for this attempt group.
    /// </summary>
    public required string ExecutionId { get; init; }

    /// <summary>
    /// Gets the idempotency key forwarded to the target service.
    /// </summary>
    public required string IdempotencyKey { get; init; }

    /// <summary>
    /// Gets the optional correlation id or trace id.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Creates a scheduler execution context.
    /// </summary>
    /// <param name="jobKey">The stable scheduler job key.</param>
    /// <param name="scheduledAt">The due timestamp.</param>
    /// <param name="executionId">The execution id. A new id is generated when omitted.</param>
    /// <param name="correlationId">The optional correlation id or trace id.</param>
    /// <returns>The execution context.</returns>
    public static SchedulerExecutionContext Create(
        string jobKey,
        DateTimeOffset scheduledAt,
        string? executionId = null,
        string? correlationId = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jobKey);

        return new SchedulerExecutionContext
        {
            JobKey = jobKey,
            ScheduledAt = scheduledAt,
            ExecutionId = string.IsNullOrWhiteSpace(executionId)
                ? Guid.NewGuid().ToString("N")
                : executionId,
            IdempotencyKey = SchedulerIdempotencyKey.Create(
                jobKey,
                scheduledAt
            ),
            CorrelationId = correlationId
        };
    }
}
