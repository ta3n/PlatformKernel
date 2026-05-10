using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.TickerQ.Abstractions;
using SharedKernel.TickerQ.Models;
using SharedKernel.TickerQ.Options;

namespace SharedKernel.TickerQ.Infrastructure;

/// <summary>
/// Default implementation of the job scheduler service.
/// </summary>
internal sealed class JobSchedulerService : IJobScheduler
{
    private readonly ITickerQSchedulerEngine _engine;
    private readonly TickerQServerOptions _serverOptions;
    private readonly ILogger<JobSchedulerService> _logger;

    public JobSchedulerService(
        ITickerQSchedulerEngine engine,
        IOptions<TickerQServerOptions> serverOptions,
        ILogger<JobSchedulerService> logger
    )
    {
        _engine = engine ?? throw new ArgumentNullException(nameof(engine));
        _serverOptions = serverOptions?.Value ?? throw new ArgumentNullException(nameof(serverOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> ScheduleTimeJobAsync(
        string function,
        DateTime runAt,
        object? request = null,
        int? retries = null,
        CancellationToken cancellationToken = default
    )
    {
        var jobId = GenerateJobId(function);
        var definition = new SchedulerJobDefinition
        {
            JobId = jobId,
            Function = function,
            JobType = SchedulerJobType.Time,
            RunAt = runAt,
            Request = request,
            Retries = retries ?? _serverOptions.DefaultRetries,
            RetryIntervalsInSeconds = _serverOptions.DefaultRetryIntervalsInSeconds
        };

        var result = await _engine.ScheduleAsync(definition, cancellationToken);

        if (!result.Success)
        {
            _logger.LogError("Failed to schedule time job: {Error}", result.ErrorMessage);
            throw new InvalidOperationException($"Failed to schedule job: {result.ErrorMessage}");
        }

        return result.JobId!;
    }

    public async Task<string> ScheduleCronJobAsync(
        string function,
        string cronExpression,
        object? request = null,
        int? retries = null,
        CancellationToken cancellationToken = default
    )
    {
        var jobId = GenerateJobId(function);
        var definition = new SchedulerJobDefinition
        {
            JobId = jobId,
            Function = function,
            JobType = SchedulerJobType.Cron,
            CronExpression = cronExpression,
            Request = request,
            Retries = retries ?? _serverOptions.DefaultRetries,
            RetryIntervalsInSeconds = _serverOptions.DefaultRetryIntervalsInSeconds
        };

        var result = await _engine.ScheduleAsync(definition, cancellationToken);

        if (!result.Success)
        {
            _logger.LogError("Failed to schedule cron job: {Error}", result.ErrorMessage);
            throw new InvalidOperationException($"Failed to schedule job: {result.ErrorMessage}");
        }

        return result.JobId!;
    }

    public async Task CancelJobAsync(
        string jobId,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _engine.CancelAsync(jobId, cancellationToken);

        if (!result.Success)
        {
            _logger.LogError("Failed to cancel job {JobId}: {Error}", jobId, result.ErrorMessage);
            throw new InvalidOperationException($"Failed to cancel job: {result.ErrorMessage}");
        }
    }

    public Task<SchedulerJobStatus?> GetJobStatusAsync(
        string jobId,
        CancellationToken cancellationToken = default
    )
    {
        return _engine.GetStatusAsync(jobId, cancellationToken);
    }

    private static string GenerateJobId(
        string function
    )
    {
        return $"{function}:{Guid.NewGuid():N}";
    }
}
