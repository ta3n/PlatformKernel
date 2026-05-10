using SharedKernel.Hangfire.Models;

namespace SharedKernel.Hangfire.Abstractions;

/// <summary>
/// Provides the Hangfire-specific scheduling operations used by the scheduler service.
/// </summary>
public interface IHangfireSchedulerEngine
{
    /// <summary>
    /// Registers or updates a recurring job that calls the generic job executor.
    /// </summary>
    /// <param name="definition">The scheduler job definition.</param>
    void RegisterRecurringJob(
        SchedulerJobDefinition definition
    );

    /// <summary>
    /// Removes a recurring job from Hangfire.
    /// </summary>
    /// <param name="jobKey">The stable scheduler job key.</param>
    void RemoveRecurringJob(
        string jobKey
    );

    /// <summary>
    /// Removes a recurring job from Hangfire while keeping metadata in application storage.
    /// </summary>
    /// <param name="jobKey">The stable scheduler job key.</param>
    void PauseRecurringJob(
        string jobKey
    );

    /// <summary>
    /// Restores a paused recurring job in Hangfire.
    /// </summary>
    /// <param name="definition">The scheduler job definition.</param>
    void ResumeRecurringJob(
        SchedulerJobDefinition definition
    );

    /// <summary>
    /// Enqueues a job for immediate execution.
    /// </summary>
    /// <param name="jobKey">The stable scheduler job key.</param>
    /// <param name="scheduledAt">The scheduler timestamp used for idempotency.</param>
    /// <param name="executionId">The execution id. A new id is generated when omitted.</param>
    /// <param name="correlationId">The optional correlation id or trace id.</param>
    /// <returns>The Hangfire background job id.</returns>
    string TriggerJobNow(
        string jobKey,
        DateTimeOffset? scheduledAt = null,
        string? executionId = null,
        string? correlationId = null
    );

    /// <summary>
    /// Schedules a one-off job at a specific timestamp.
    /// </summary>
    /// <param name="definition">The scheduler job definition.</param>
    /// <param name="scheduledAt">The timestamp when the job should be due.</param>
    /// <returns>The Hangfire background job id.</returns>
    string ScheduleJobAt(
        SchedulerJobDefinition definition,
        DateTimeOffset scheduledAt
    );

    /// <summary>
    /// Schedules a one-off job after a delay.
    /// </summary>
    /// <param name="definition">The scheduler job definition.</param>
    /// <param name="delay">The delay before execution.</param>
    /// <returns>The Hangfire background job id.</returns>
    string ScheduleJobDelay(
        SchedulerJobDefinition definition,
        TimeSpan delay
    );
}
