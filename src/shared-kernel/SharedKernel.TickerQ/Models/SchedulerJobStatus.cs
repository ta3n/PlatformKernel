namespace SharedKernel.TickerQ.Models;

/// <summary>
/// Represents the current status of a scheduled job.
/// </summary>
public sealed class SchedulerJobStatus
{
    /// <summary>
    /// The job identifier.
    /// </summary>
    public string JobId { get; set; } = string.Empty;

    /// <summary>
    /// The function name.
    /// </summary>
    public string Function { get; set; } = string.Empty;

    /// <summary>
    /// Current job state.
    /// </summary>
    public JobState State { get; set; }

    /// <summary>
    /// Next scheduled execution time (for cron jobs).
    /// </summary>
    public DateTime? NextRunAt { get; set; }

    /// <summary>
    /// Last execution time.
    /// </summary>
    public DateTime? LastExecutedAt { get; set; }

    /// <summary>
    /// Number of execution attempts.
    /// </summary>
    public int AttemptCount { get; set; }

    /// <summary>
    /// Error message from last failed execution.
    /// </summary>
    public string? LastError { get; set; }
}

/// <summary>
/// Defines possible job states.
/// </summary>
public enum JobState
{
    Scheduled,
    Running,
    Completed,
    Failed,
    Cancelled
}
