using Microsoft.Extensions.Logging;
using SharedKernel.Hangfire.Abstractions;
using SharedKernel.Hangfire.Exceptions;
using SharedKernel.Hangfire.Models;
using SharedKernel.Hangfire.Utils;

namespace SharedKernel.Hangfire.Infrastructure;

/// <summary>
/// Application-level scheduler orchestration used by gRPC handlers.
/// </summary>
public sealed class JobSchedulerService(
    IJobMetadataStore metadataStore,
    IHangfireSchedulerEngine schedulerEngine,
    ILogger<JobSchedulerService> logger
) : IJobScheduler
{
    private readonly IJobMetadataStore _metadataStore =
        metadataStore ?? throw new ArgumentNullException(nameof(metadataStore));

    private readonly IHangfireSchedulerEngine _schedulerEngine =
        schedulerEngine ?? throw new ArgumentNullException(nameof(schedulerEngine));

    private readonly ILogger<JobSchedulerService> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<SchedulerOperationResult> RegisterRecurringJobAsync(
        SchedulerJobDefinition definition,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(definition);
        SchedulerJobDefinitionValidator.EnsureValidRecurring(definition);

        var activeDefinition = definition with { Status = SchedulerJobStatus.Active };

        await _metadataStore.UpsertAsync(
            activeDefinition,
            cancellationToken
        );

        _schedulerEngine.RegisterRecurringJob(activeDefinition);

        _logger.LogInformation(
            "Registered recurring scheduler job {JobKey} for {TargetService}.{GrpcMethod}",
            activeDefinition.JobKey,
            activeDefinition.TargetService,
            activeDefinition.GrpcMethod
        );

        return SchedulerOperationResult.Success(
            activeDefinition.JobKey,
            message: "Recurring job registered."
        );
    }

    /// <inheritdoc />
    public async Task<SchedulerOperationResult> RemoveRecurringJobAsync(
        string jobKey,
        CancellationToken cancellationToken = default
    )
    {
        var validatedJobKey = SchedulerJobDefinitionValidator.EnsureJobKey(jobKey);

        _schedulerEngine.RemoveRecurringJob(validatedJobKey);

        await _metadataStore.SetStatusAsync(
            validatedJobKey,
            SchedulerJobStatus.Removed,
            cancellationToken
        );

        _logger.LogInformation(
            "Removed recurring scheduler job {JobKey}",
            validatedJobKey
        );

        return SchedulerOperationResult.Success(
            validatedJobKey,
            message: "Recurring job removed."
        );
    }

    /// <inheritdoc />
    public async Task<SchedulerOperationResult> TriggerJobNowAsync(
        string jobKey,
        string requestedBy,
        string? correlationId = null,
        CancellationToken cancellationToken = default
    )
    {
        var validatedJobKey = SchedulerJobDefinitionValidator.EnsureJobKey(jobKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(requestedBy);

        var definition = await GetExistingDefinitionAsync(
            validatedJobKey,
            cancellationToken
        );

        if (definition.Status != SchedulerJobStatus.Active)
        {
            throw new InvalidSchedulerJobDefinitionException(
                $"Scheduler job '{validatedJobKey}' is {definition.Status} and cannot be triggered."
            );
        }

        var hangfireJobId = _schedulerEngine.TriggerJobNow(
            validatedJobKey,
            correlationId: correlationId
        );

        _logger.LogInformation(
            "Triggered scheduler job {JobKey} on demand by {RequestedBy}",
            validatedJobKey,
            requestedBy
        );

        return SchedulerOperationResult.Success(
            validatedJobKey,
            hangfireJobId,
            "Job triggered."
        );
    }

    /// <inheritdoc />
    public async Task<SchedulerOperationResult> PauseJobAsync(
        string jobKey,
        string requestedBy,
        CancellationToken cancellationToken = default
    )
    {
        var validatedJobKey = SchedulerJobDefinitionValidator.EnsureJobKey(jobKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(requestedBy);

        var definition = await GetExistingDefinitionAsync(
            validatedJobKey,
            cancellationToken
        );

        if (definition.Status != SchedulerJobStatus.Active)
        {
            throw new InvalidSchedulerJobDefinitionException(
                $"Scheduler job '{validatedJobKey}' is {definition.Status} and cannot be paused."
            );
        }

        _schedulerEngine.PauseRecurringJob(validatedJobKey);

        await _metadataStore.SetStatusAsync(
            validatedJobKey,
            SchedulerJobStatus.Paused,
            cancellationToken
        );

        _logger.LogInformation(
            "Paused scheduler job {JobKey} by {RequestedBy}",
            validatedJobKey,
            requestedBy
        );

        return SchedulerOperationResult.Success(
            validatedJobKey,
            message: "Job paused."
        );
    }

    /// <inheritdoc />
    public async Task<SchedulerOperationResult> ResumeJobAsync(
        string jobKey,
        string requestedBy,
        CancellationToken cancellationToken = default
    )
    {
        var validatedJobKey = SchedulerJobDefinitionValidator.EnsureJobKey(jobKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(requestedBy);

        var definition = await GetExistingDefinitionAsync(
            validatedJobKey,
            cancellationToken
        );

        var activeDefinition = definition with { Status = SchedulerJobStatus.Active };

        SchedulerJobDefinitionValidator.EnsureValidRecurring(activeDefinition);

        await _metadataStore.UpsertAsync(
            activeDefinition,
            cancellationToken
        );

        _schedulerEngine.ResumeRecurringJob(activeDefinition);

        _logger.LogInformation(
            "Resumed scheduler job {JobKey} by {RequestedBy}",
            validatedJobKey,
            requestedBy
        );

        return SchedulerOperationResult.Success(
            validatedJobKey,
            message: "Job resumed."
        );
    }

    /// <inheritdoc />
    public async Task<SchedulerOperationResult> ScheduleJobAtAsync(
        SchedulerJobDefinition definition,
        DateTimeOffset scheduledAt,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(definition);
        SchedulerJobDefinitionValidator.EnsureValidTarget(definition);

        var activeDefinition = definition with { Status = SchedulerJobStatus.Active };

        await _metadataStore.UpsertAsync(
            activeDefinition,
            cancellationToken
        );

        var hangfireJobId = _schedulerEngine.ScheduleJobAt(
            activeDefinition,
            scheduledAt
        );

        _logger.LogInformation(
            "Scheduled one-off job {JobKey} at {ScheduledAt} for {TargetService}.{GrpcMethod}",
            activeDefinition.JobKey,
            scheduledAt,
            activeDefinition.TargetService,
            activeDefinition.GrpcMethod
        );

        return SchedulerOperationResult.Success(
            activeDefinition.JobKey,
            hangfireJobId,
            "Job scheduled at specific time."
        );
    }

    /// <inheritdoc />
    public async Task<SchedulerOperationResult> ScheduleJobDelayAsync(
        SchedulerJobDefinition definition,
        TimeSpan delay,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(definition);
        SchedulerJobDefinitionValidator.EnsureValidTarget(definition);

        var activeDefinition = definition with { Status = SchedulerJobStatus.Active };

        await _metadataStore.UpsertAsync(
            activeDefinition,
            cancellationToken
        );

        var hangfireJobId = _schedulerEngine.ScheduleJobDelay(
            activeDefinition,
            delay
        );

        _logger.LogInformation(
            "Scheduled one-off job {JobKey} with delay {Delay} for {TargetService}.{GrpcMethod}",
            activeDefinition.JobKey,
            delay,
            activeDefinition.TargetService,
            activeDefinition.GrpcMethod
        );

        return SchedulerOperationResult.Success(
            activeDefinition.JobKey,
            hangfireJobId,
            "Job scheduled with delay."
        );
    }

    private async Task<SchedulerJobDefinition> GetExistingDefinitionAsync(
        string jobKey,
        CancellationToken cancellationToken
    )
    {
        var definition = await _metadataStore.GetByJobKeyAsync(
            jobKey,
            cancellationToken
        );

        return definition
            ?? throw new InvalidSchedulerJobDefinitionException(
                $"Scheduler job '{jobKey}' was not found."
            );
    }
}
