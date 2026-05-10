using SharedKernel.TickerQ.Models;

namespace SharedKernel.TickerQ.Abstractions;

/// <summary>
/// High-level job scheduler service for application-level job management.
/// </summary>
public interface IJobScheduler
{
    /// <summary>
    /// Schedules a time-based job to execute once at a specific time.
    /// </summary>
    /// <param name="function">The function name (must match [TickerFunction] attribute).</param>
    /// <param name="runAt">The UTC execution time.</param>
    /// <param name="request">Optional request payload.</param>
    /// <param name="retries">Number of retry attempts.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The scheduled job ID.</returns>
    Task<string> ScheduleTimeJobAsync(
        string function,
        DateTime runAt,
        object? request = null,
        int? retries = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Schedules a recurring job using a cron expression.
    /// </summary>
    /// <param name="function">The function name (must match [TickerFunction] attribute).</param>
    /// <param name="cronExpression">The cron expression (e.g., "0 */6 * * *").</param>
    /// <param name="request">Optional request payload.</param>
    /// <param name="retries">Number of retry attempts.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The scheduled job ID.</returns>
    Task<string> ScheduleCronJobAsync(
        string function,
        string cronExpression,
        object? request = null,
        int? retries = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Cancels a scheduled job.
    /// </summary>
    /// <param name="jobId">The job identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task CancelJobAsync(
        string jobId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets the status of a scheduled job.
    /// </summary>
    /// <param name="jobId">The job identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The job status, or null if not found.</returns>
    Task<SchedulerJobStatus?> GetJobStatusAsync(
        string jobId,
        CancellationToken cancellationToken = default
    );
}
