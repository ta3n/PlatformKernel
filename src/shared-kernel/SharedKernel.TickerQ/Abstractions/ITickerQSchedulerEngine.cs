using SharedKernel.TickerQ.Models;

namespace SharedKernel.TickerQ.Abstractions;

/// <summary>
/// Core scheduler engine for managing TickerQ jobs.
/// </summary>
public interface ITickerQSchedulerEngine
{
    /// <summary>
    /// Schedules a new job based on the provided definition.
    /// </summary>
    /// <param name="definition">The job definition.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The operation result containing the scheduled job ID.</returns>
    Task<SchedulerOperationResult> ScheduleAsync(
        SchedulerJobDefinition definition,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Cancels a scheduled job by ID.
    /// </summary>
    /// <param name="jobId">The job identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The operation result.</returns>
    Task<SchedulerOperationResult> CancelAsync(
        string jobId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves the status of a scheduled job.
    /// </summary>
    /// <param name="jobId">The job identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The job status, or null if not found.</returns>
    Task<SchedulerJobStatus?> GetStatusAsync(
        string jobId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Reschedules an existing job with a new execution time or cron expression.
    /// </summary>
    /// <param name="jobId">The job identifier.</param>
    /// <param name="definition">The updated job definition.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The operation result.</returns>
    Task<SchedulerOperationResult> RescheduleAsync(
        string jobId,
        SchedulerJobDefinition definition,
        CancellationToken cancellationToken = default
    );
}
