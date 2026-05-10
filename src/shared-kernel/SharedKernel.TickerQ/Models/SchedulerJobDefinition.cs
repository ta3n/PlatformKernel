namespace SharedKernel.TickerQ.Models;

/// <summary>
/// Represents a job definition for scheduling with TickerQ.
/// </summary>
public sealed class SchedulerJobDefinition
{
    /// <summary>
    /// Unique identifier for the scheduled job.
    /// </summary>
    public string JobId { get; set; } = string.Empty;

    /// <summary>
    /// The function name to execute (must match [TickerFunction] attribute).
    /// </summary>
    public string Function { get; set; } = string.Empty;

    /// <summary>
    /// Job execution type: "Time" or "Cron".
    /// </summary>
    public SchedulerJobType JobType { get; set; } = SchedulerJobType.Time;

    /// <summary>
    /// Execution time for time-based jobs (UTC).
    /// </summary>
    public DateTime? RunAt { get; set; }

    /// <summary>
    /// Cron expression for recurring jobs (e.g., "0 */6 * * *").
    /// </summary>
    public string? CronExpression { get; set; }

    /// <summary>
    /// JSON payload passed to the job function.
    /// </summary>
    public object? Request { get; set; }

    /// <summary>
    /// Number of retry attempts on failure.
    /// </summary>
    public int Retries { get; set; } = 3;

    /// <summary>
    /// Retry intervals in seconds.
    /// </summary>
    public int[] RetryIntervalsInSeconds { get; set; } = [60, 120, 300];

    /// <summary>
    /// Optional description of the job.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Optional tags for categorizing jobs.
    /// </summary>
    public string[]? Tags { get; set; }
}

/// <summary>
/// Defines job execution types.
/// </summary>
public enum SchedulerJobType
{
    Time,
    Cron
}
