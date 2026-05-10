namespace SharedKernel.Hangfire.Models;

/// <summary>
/// Represents the scheduler lifecycle status for a job definition.
/// </summary>
public enum SchedulerJobStatus
{
    /// <summary>
    /// The job is active and can be scheduled.
    /// </summary>
    Active = 0,

    /// <summary>
    /// The job is paused and should not be scheduled automatically.
    /// </summary>
    Paused = 1,

    /// <summary>
    /// The job has been removed.
    /// </summary>
    Removed = 2
}
