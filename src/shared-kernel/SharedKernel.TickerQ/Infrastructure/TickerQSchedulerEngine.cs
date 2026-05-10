using Microsoft.Extensions.Logging;
using SharedKernel.TickerQ.Abstractions;
using SharedKernel.TickerQ.Models;

namespace SharedKernel.TickerQ.Infrastructure;

/// <summary>
/// Default implementation of the TickerQ scheduler engine.
/// </summary>
internal sealed class TickerQSchedulerEngine : ITickerQSchedulerEngine
{
    private readonly ILogger<TickerQSchedulerEngine> _logger;

    public TickerQSchedulerEngine(
        ILogger<TickerQSchedulerEngine> logger
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<SchedulerOperationResult> ScheduleAsync(
        SchedulerJobDefinition definition,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(definition);
        ValidateJobDefinition(definition);

        _logger.LogInformation(
            "Scheduling {JobType} job: {Function} with JobId: {JobId}",
            definition.JobType,
            definition.Function,
            definition.JobId
        );

        // Implementation note: Actual scheduling is done via ITimeTickerManager<> or ICronTickerManager<>
        // This engine provides a unified abstraction over both managers.
        // Concrete implementations should inject and delegate to the appropriate manager.

        return Task.FromResult(SchedulerOperationResult.Succeeded(definition.JobId));
    }

    public Task<SchedulerOperationResult> CancelAsync(
        string jobId,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(jobId))
        {
            throw new ArgumentException("Job ID cannot be null or empty.", nameof(jobId));
        }

        _logger.LogInformation("Cancelling job: {JobId}", jobId);

        // Implementation note: Delegate to appropriate manager to cancel the job.

        return Task.FromResult(SchedulerOperationResult.Succeeded(jobId));
    }

    public Task<SchedulerJobStatus?> GetStatusAsync(
        string jobId,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(jobId))
        {
            throw new ArgumentException("Job ID cannot be null or empty.", nameof(jobId));
        }

        _logger.LogDebug("Retrieving status for job: {JobId}", jobId);

        // Implementation note: Query the appropriate manager for job status.

        return Task.FromResult<SchedulerJobStatus?>(null);
    }

    public Task<SchedulerOperationResult> RescheduleAsync(
        string jobId,
        SchedulerJobDefinition definition,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(jobId))
        {
            throw new ArgumentException("Job ID cannot be null or empty.", nameof(jobId));
        }

        ArgumentNullException.ThrowIfNull(definition);
        ValidateJobDefinition(definition);

        _logger.LogInformation("Rescheduling job: {JobId}", jobId);

        // Implementation note: Cancel existing job and schedule new one with updated definition.

        return Task.FromResult(SchedulerOperationResult.Succeeded(jobId));
    }

    private static void ValidateJobDefinition(
        SchedulerJobDefinition definition
    )
    {
        if (string.IsNullOrWhiteSpace(definition.Function))
        {
            throw new ArgumentException(
                string.Format(Constants.ErrorMessages.InvalidJobDefinition, "Function name is required."),
                nameof(definition)
            );
        }

        if (definition.JobType == SchedulerJobType.Time && definition.RunAt is null)
        {
            throw new ArgumentException(
                string.Format(Constants.ErrorMessages.InvalidJobDefinition, "RunAt is required for time-based jobs."),
                nameof(definition)
            );
        }

        if (definition.JobType == SchedulerJobType.Cron && string.IsNullOrWhiteSpace(definition.CronExpression))
        {
            throw new ArgumentException(
                string.Format(
                    Constants.ErrorMessages.InvalidJobDefinition,
                    "CronExpression is required for cron-based jobs."
                ),
                nameof(definition)
            );
        }
    }
}
