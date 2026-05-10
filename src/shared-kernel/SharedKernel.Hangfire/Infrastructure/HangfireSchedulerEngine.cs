using Hangfire;
using SharedKernel.Hangfire.Abstractions;
using SharedKernel.Hangfire.Models;
using SharedKernel.Hangfire.Utils;

namespace SharedKernel.Hangfire.Infrastructure;

/// <summary>
/// Schedules generic scheduler executor jobs through Hangfire.
/// </summary>
public sealed class HangfireSchedulerEngine(
    IRecurringJobManager recurringJobManager,
    IBackgroundJobClient backgroundJobClient
) : IHangfireSchedulerEngine
{
    private readonly IRecurringJobManager _recurringJobManager =
        recurringJobManager ?? throw new ArgumentNullException(nameof(recurringJobManager));

    private readonly IBackgroundJobClient _backgroundJobClient =
        backgroundJobClient ?? throw new ArgumentNullException(nameof(backgroundJobClient));

    /// <inheritdoc />
    public void RegisterRecurringJob(
        SchedulerJobDefinition definition
    )
    {
        SchedulerJobDefinitionValidator.EnsureValidRecurring(definition);

        _recurringJobManager.AddOrUpdate<IJobExecutor>(
            definition.JobKey,
            executor => executor.ExecuteRecurringAsync(
                definition.JobKey,
                null,
                CancellationToken.None
            ),
            definition.CronExpression!,
            JobUtil.GetRecurringJobOptions(definition.TimeZoneId)
        );
    }

    /// <inheritdoc />
    public void RemoveRecurringJob(
        string jobKey
    )
    {
        _recurringJobManager.RemoveIfExists(
            SchedulerJobDefinitionValidator.EnsureJobKey(jobKey)
        );
    }

    /// <inheritdoc />
    public void PauseRecurringJob(
        string jobKey
    )
    {
        RemoveRecurringJob(jobKey);
    }

    /// <inheritdoc />
    public void ResumeRecurringJob(
        SchedulerJobDefinition definition
    )
    {
        RegisterRecurringJob(definition);
    }

    /// <inheritdoc />
    public string TriggerJobNow(
        string jobKey,
        DateTimeOffset? scheduledAt = null,
        string? executionId = null,
        string? correlationId = null
    )
    {
        var request = SchedulerJobExecutionRequest.Create(
            SchedulerJobDefinitionValidator.EnsureJobKey(jobKey),
            scheduledAt ?? DateTimeOffset.UtcNow,
            executionId,
            correlationId
        );

        return _backgroundJobClient.Enqueue<IJobExecutor>(
            executor => executor.ExecuteAsync(
                request,
                CancellationToken.None
            )
        );
    }

    /// <inheritdoc />
    public string ScheduleJobAt(
        SchedulerJobDefinition definition,
        DateTimeOffset scheduledAt
    )
    {
        SchedulerJobDefinitionValidator.EnsureValidTarget(definition);

        var request = SchedulerJobExecutionRequest.Create(
            definition.JobKey,
            scheduledAt
        );

        return _backgroundJobClient.Schedule<IJobExecutor>(
            executor => executor.ExecuteAsync(
                request,
                CancellationToken.None
            ),
            scheduledAt
        );
    }

    /// <inheritdoc />
    public string ScheduleJobDelay(
        SchedulerJobDefinition definition,
        TimeSpan delay
    )
    {
        if (delay <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(delay),
                "Schedule delay must be greater than zero."
            );
        }

        SchedulerJobDefinitionValidator.EnsureValidTarget(definition);

        var scheduledAt = DateTimeOffset.UtcNow.Add(delay);
        var request = SchedulerJobExecutionRequest.Create(
            definition.JobKey,
            scheduledAt
        );

        return _backgroundJobClient.Schedule<IJobExecutor>(
            executor => executor.ExecuteAsync(
                request,
                CancellationToken.None
            ),
            delay
        );
    }
}
