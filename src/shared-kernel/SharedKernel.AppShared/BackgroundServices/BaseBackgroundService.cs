using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SharedKernel.AppShared.BackgroundServices;

public abstract class BaseBackgroundService<TJob> : BackgroundService
{
    private readonly Channel<TJob> _queue;
    private readonly ChannelReader<TJob> _reader;

    private readonly ILogger<BaseBackgroundService<TJob>> _logger;

    protected readonly IServiceProvider ServiceProvider;

    private const int MaxConcurrentWorkers = 3;
    private const int MaxQueueCapacity = 5000;

    protected BaseBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<BaseBackgroundService<TJob>> logger
    )
    {
        ServiceProvider = serviceProvider;
        _logger = logger;

        var options = new BoundedChannelOptions(MaxQueueCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        };

        _queue = Channel.CreateBounded<TJob>(options);
        _reader = _queue.Reader;
    }

    public async ValueTask EnqueueEmailJobAsync(
        TJob job,
        CancellationToken cancellationToken = default
    )
    {
        // Check if we're under the high load
        if (_queue.Reader.Count > 4000) // 80% of capacity
        {
            _logger.LogWarning(
                "Email queue is near capacity ({Count}/5000). Consider scaling or dropping low-priority emails",
                _queue.Reader.Count
            );
        }

        var success = _queue.Writer.TryWrite(job);
        if (!success)
        {
            try
            {
                await _queue.Writer.WriteAsync(job, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to enqueue job - channel closed: {Message}",
                    ex.Message
                );
                // Exception is already logged, do not rethrow
            }
        }
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken
    )
    {
        var workers = Enumerable.Range(0, MaxConcurrentWorkers)
            .Select(i => ProcessEmailWorkerAsync(i, stoppingToken))
            .ToArray();

        await Task.WhenAll(workers);
    }

    private async Task ProcessEmailWorkerAsync(
        int workerId,
        CancellationToken stoppingToken
    )
    {
        _logger.LogInformation("Email worker {WorkerId} started", workerId);

        await foreach (var job in _reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await ProcessJobAsync(job, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Worker {WorkerId} failed to process job: {Message}",
                    workerId,
                    ex.Message
                );
            }
        }

        _logger.LogInformation("Email worker {WorkerId} stopped", workerId);
    }

    protected abstract Task ProcessJobAsync(
        TJob job,
        CancellationToken cancellationToken
    );
}
