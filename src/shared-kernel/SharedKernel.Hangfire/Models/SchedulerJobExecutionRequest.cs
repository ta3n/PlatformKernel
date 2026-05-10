namespace SharedKernel.Hangfire.Models;

/// <summary>
/// Represents a concrete Hangfire execution request for a scheduler job.
/// </summary>
public sealed record SchedulerJobExecutionRequest
{
    /// <summary>
    /// Gets the stable scheduler job key.
    /// </summary>
    public required string JobKey { get; init; }

    /// <summary>
    /// Gets the due timestamp used to compute idempotency.
    /// </summary>
    public required DateTimeOffset ScheduledAt { get; init; }

    /// <summary>
    /// Gets the unique execution id for this occurrence.
    /// </summary>
    public required string ExecutionId { get; init; }

    /// <summary>
    /// Gets the optional correlation id or trace id.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Creates an execution request.
    /// </summary>
    /// <param name="jobKey">The stable scheduler job key.</param>
    /// <param name="scheduledAt">The due timestamp.</param>
    /// <param name="executionId">The execution id. A new id is generated when omitted.</param>
    /// <param name="correlationId">The optional correlation id or trace id.</param>
    /// <returns>The execution request.</returns>
    public static SchedulerJobExecutionRequest Create(
        string jobKey,
        DateTimeOffset scheduledAt,
        string? executionId = null,
        string? correlationId = null
    )
    {
        var execution = SchedulerExecutionContext.Create(
            jobKey,
            scheduledAt,
            executionId,
            correlationId
        );

        return new SchedulerJobExecutionRequest
        {
            JobKey = execution.JobKey,
            ScheduledAt = execution.ScheduledAt,
            ExecutionId = execution.ExecutionId,
            CorrelationId = execution.CorrelationId
        };
    }

    /// <summary>
    /// Converts this request to execution metadata.
    /// </summary>
    /// <returns>The execution context.</returns>
    public SchedulerExecutionContext ToExecutionContext()
    {
        return SchedulerExecutionContext.Create(
            JobKey,
            ScheduledAt,
            ExecutionId,
            CorrelationId
        );
    }
}
