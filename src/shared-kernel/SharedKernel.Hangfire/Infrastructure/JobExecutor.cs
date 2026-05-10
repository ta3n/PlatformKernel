using Hangfire.Server;
using Microsoft.Extensions.Logging;
using SharedKernel.Hangfire.Abstractions;
using SharedKernel.Hangfire.Exceptions;
using SharedKernel.Hangfire.Models;
using SharedKernel.Hangfire.Utils;

namespace SharedKernel.Hangfire.Infrastructure;

/// <summary>
/// Generic scheduler executor that loads job metadata and dispatches the target gRPC call.
/// </summary>
public sealed class JobExecutor(
    IJobMetadataStore metadataStore,
    IGrpcDispatcher grpcDispatcher,
    ILogger<JobExecutor> logger
) : IJobExecutor
{
    private readonly IJobMetadataStore _metadataStore =
        metadataStore ?? throw new ArgumentNullException(nameof(metadataStore));

    private readonly IGrpcDispatcher _grpcDispatcher =
        grpcDispatcher ?? throw new ArgumentNullException(nameof(grpcDispatcher));

    private readonly ILogger<JobExecutor> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task ExecuteRecurringAsync(
        string jobKey,
        PerformContext? performContext = null,
        CancellationToken cancellationToken = default
    )
    {
        var hangfireJob = performContext?.BackgroundJob;
        var scheduledAt = TruncateToMinute(hangfireJob?.CreatedAt ?? DateTimeOffset.UtcNow);
        var request = SchedulerJobExecutionRequest.Create(
            jobKey,
            scheduledAt,
            hangfireJob?.Id,
            hangfireJob?.Id
        );

        await ExecuteAsync(
            request,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task ExecuteAsync(
        SchedulerJobExecutionRequest request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        var execution = request.ToExecutionContext();

        using var scope = _logger.BeginScope(
            new Dictionary<string, object?>
            {
                [nameof(execution.JobKey)] = execution.JobKey,
                [nameof(execution.ExecutionId)] = execution.ExecutionId,
                [nameof(execution.IdempotencyKey)] = execution.IdempotencyKey,
                [nameof(execution.CorrelationId)] = execution.CorrelationId
            }
        );

        var definition = await _metadataStore.GetByJobKeyAsync(
            execution.JobKey,
            cancellationToken
        );

        if (definition is null)
        {
            throw new InvalidSchedulerJobDefinitionException(
                $"Scheduler job '{execution.JobKey}' was not found."
            );
        }

        if (definition.Status is SchedulerJobStatus.Paused or SchedulerJobStatus.Removed)
        {
            _logger.LogInformation(
                "Skipping scheduler job {JobKey} because status is {Status}",
                definition.JobKey,
                definition.Status
            );

            return;
        }

        SchedulerJobDefinitionValidator.EnsureValidTarget(definition);

        var dispatchRequest = SchedulerGrpcDispatchRequest.Create(
            definition,
            execution
        );

        _logger.LogInformation(
            "Dispatching scheduler job {JobKey} to {TargetService}.{GrpcMethod}",
            definition.JobKey,
            definition.TargetService,
            definition.GrpcMethod
        );

        await _grpcDispatcher.DispatchAsync(
            dispatchRequest,
            cancellationToken
        );
    }

    private static DateTimeOffset TruncateToMinute(
        DateTimeOffset value
    )
    {
        var utc = value.ToUniversalTime();

        return new DateTimeOffset(
            utc.Year,
            utc.Month,
            utc.Day,
            utc.Hour,
            utc.Minute,
            0,
            TimeSpan.Zero
        );
    }
}
