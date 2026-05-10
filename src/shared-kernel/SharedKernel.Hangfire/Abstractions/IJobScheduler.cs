using SharedKernel.Hangfire.Models;

namespace SharedKernel.Hangfire.Abstractions;

/// <summary>
/// Application-level scheduler service contract used by gRPC handlers.
/// </summary>
public interface IJobScheduler
{
    /// <summary>
    /// Registers or updates a recurring scheduler job.
    /// </summary>
    /// <param name="definition">The job definition.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The operation result.</returns>
    Task<SchedulerOperationResult> RegisterRecurringJobAsync(
        SchedulerJobDefinition definition,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Removes a recurring scheduler job.
    /// </summary>
    /// <param name="jobKey">The stable scheduler job key.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The operation result.</returns>
    Task<SchedulerOperationResult> RemoveRecurringJobAsync(
        string jobKey,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Triggers a scheduler job immediately.
    /// </summary>
    /// <param name="jobKey">The stable scheduler job key.</param>
    /// <param name="requestedBy">The caller that requested the trigger.</param>
    /// <param name="correlationId">The optional correlation id or trace id.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The operation result.</returns>
    Task<SchedulerOperationResult> TriggerJobNowAsync(
        string jobKey,
        string requestedBy,
        string? correlationId = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Pauses a scheduler job.
    /// </summary>
    /// <param name="jobKey">The stable scheduler job key.</param>
    /// <param name="requestedBy">The caller that requested the pause.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The operation result.</returns>
    Task<SchedulerOperationResult> PauseJobAsync(
        string jobKey,
        string requestedBy,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Resumes a scheduler job.
    /// </summary>
    /// <param name="jobKey">The stable scheduler job key.</param>
    /// <param name="requestedBy">The caller that requested the resume.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The operation result.</returns>
    Task<SchedulerOperationResult> ResumeJobAsync(
        string jobKey,
        string requestedBy,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Schedules a one-off job to execute at a specific timestamp.
    /// </summary>
    /// <param name="definition">The job definition.</param>
    /// <param name="scheduledAt">The timestamp when the job should be executed.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The operation result.</returns>
    Task<SchedulerOperationResult> ScheduleJobAtAsync(
        SchedulerJobDefinition definition,
        DateTimeOffset scheduledAt,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Schedules a one-off job to execute after a delay.
    /// </summary>
    /// <param name="definition">The job definition.</param>
    /// <param name="delay">The delay before execution.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The operation result.</returns>
    Task<SchedulerOperationResult> ScheduleJobDelayAsync(
        SchedulerJobDefinition definition,
        TimeSpan delay,
        CancellationToken cancellationToken = default
    );
}
