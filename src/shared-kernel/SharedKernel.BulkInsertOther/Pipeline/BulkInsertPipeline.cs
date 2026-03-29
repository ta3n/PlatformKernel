using System.Buffers;
using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedKernel.BulkInsertOther.Mapping;
using SharedKernel.BulkInsertOther.Observability;
using SharedKernel.BulkInsertOther.Options;

namespace SharedKernel.BulkInsertOther.Pipeline;

internal sealed class BulkInsertPipeline<T> :
    IBulkInsertPipeline<T>,
    IHostedService
{
    private readonly BulkInsertOptions _options;
    private readonly Channel<T> _channel;
    private readonly IBulkInsertService<T> _bulkInsertService;
    private readonly IBulkInsertFailureSink<T> _failureSink;
    private readonly BulkInsertTelemetry _telemetry;
    private readonly BulkInsertEntityDescriptor<T> _descriptor;
    private readonly ILogger<BulkInsertPipeline<T>> _logger;
    private readonly Task[] _workers;
    private readonly CancellationTokenSource _shutdown = new();
    private long _queueDepth;
    private volatile bool _acceptingWrites;

    public BulkInsertPipeline(
        BulkInsertOptions options,
        IBulkInsertService<T> bulkInsertService,
        IBulkInsertFailureSink<T> failureSink,
        BulkInsertTelemetry telemetry,
        BulkInsertEntityDescriptor<T> descriptor,
        ILogger<BulkInsertPipeline<T>> logger
    )
    {
        _options = options;
        _bulkInsertService = bulkInsertService;
        _failureSink = failureSink;
        _telemetry = telemetry;
        _descriptor = descriptor;
        _logger = logger;

        _channel = Channel.CreateBounded<T>(
            new BoundedChannelOptions(_options.ChannelCapacity)
            {
                AllowSynchronousContinuations = false,
                SingleReader = _options.WorkerCount == 1,
                SingleWriter = false,
                // Keep the channel in wait mode and make overload decisions explicitly in EnqueueAsync.
                // BoundedChannelFullMode.DropWrite can report success while discarding the item, which
                // prevents us from parking or counting rejected writes deterministically.
                FullMode = BoundedChannelFullMode.Wait
            }
        );

        _workers = new Task[_options.WorkerCount];
    }

    public ValueTask EnqueueAsync(
        T item,
        CancellationToken cancellationToken = default
    )
    {
        if (_acceptingWrites)
        {
            return _options.EnableBackpressure || _options.OverloadStrategy == BulkInsertOverloadStrategy.Wait
                ? WriteWithBackpressureAsync(item, cancellationToken)
                : TryWriteAsync(item, cancellationToken);
        }

        _telemetry.RecordRejected(_descriptor.EntityName, BulkInsertRejectionReason.ChannelClosed);
        return _failureSink.OnRejectedAsync(item, BulkInsertRejectionReason.ChannelClosed, cancellationToken);
    }

    public Task StartAsync(
        CancellationToken cancellationToken
    )
    {
        _acceptingWrites = true;

        for (var workerIndex = 0; workerIndex < _workers.Length; workerIndex++)
        {
            _workers[workerIndex] = RunWorkerAsync(workerIndex, _shutdown.Token);
        }

        _logger.LogInformation(
            "Started bulk insert pipeline for entity {EntityName} with {WorkerCount} workers and channel capacity {ChannelCapacity}.",
            _descriptor.EntityName,
            _options.WorkerCount,
            _options.ChannelCapacity
        );

        return Task.CompletedTask;
    }

    public async Task StopAsync(
        CancellationToken cancellationToken
    )
    {
        _acceptingWrites = false;
        _channel.Writer.TryComplete();

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(_options.ShutdownDrainTimeout);

        try
        {
            await Task.WhenAll(_workers).WaitAsync(timeoutCts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException exception)
        {
            await _shutdown.CancelAsync().ConfigureAwait(false);
            _logger.LogWarning(
                exception,
                "Bulk insert pipeline for entity {EntityName} did not drain within {DrainTimeout}. Remaining queued items may be abandoned.",
                _descriptor.EntityName,
                _options.ShutdownDrainTimeout
            );
        }
        finally
        {
            _shutdown.Dispose();
        }
    }

    private async ValueTask WriteWithBackpressureAsync(
        T item,
        CancellationToken ct
    )
    {
        await _channel.Writer.WriteAsync(item, ct).ConfigureAwait(false);
        Interlocked.Increment(ref _queueDepth);
    }

    private async ValueTask TryWriteAsync(
        T item,
        CancellationToken ct
    )
    {
        if (_channel.Writer.TryWrite(item))
        {
            Interlocked.Increment(ref _queueDepth);
            return;
        }

        var reason = BulkInsertRejectionReason.CapacityExceeded;
        _telemetry.RecordRejected(_descriptor.EntityName, reason);

        if (_options.OverloadStrategy == BulkInsertOverloadStrategy.Park)
        {
            await _failureSink.OnRejectedAsync(item, reason, ct).ConfigureAwait(false);
        }
    }

    private async Task RunWorkerAsync(
        int workerIndex,
        CancellationToken ct
    )
    {
        var sizer = new AdaptiveBatchSizer(_options.AdaptiveBatching, _options.ChannelCapacity);

        while (await _channel.Reader.WaitToReadAsync(ct).ConfigureAwait(false))
        {
            using var lease = await ReadBatchAsync(sizer, ct).ConfigureAwait(false);
            if (lease is null || lease.Count == 0)
            {
                continue;
            }

            try
            {
                await _bulkInsertService.InsertAsync(lease.Memory, ct).ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                _logger.LogError(
                    exception,
                    "Bulk insert worker {WorkerIndex} failed to persist a batch of {BatchSize} rows for entity {EntityName}.",
                    workerIndex,
                    lease.Count,
                    _descriptor.EntityName
                );

                await _failureSink.OnBatchFailedAsync(lease.Memory, exception, ct).ConfigureAwait(false);
            }
        }
    }

    private async ValueTask<BatchLease<T>?> ReadBatchAsync(
        AdaptiveBatchSizer sizer,
        CancellationToken ct
    )
    {
        var targetSize = sizer.GetNextSize((int)Math.Min(int.MaxValue, Interlocked.Read(ref _queueDepth)));
        var lease = new BatchLease<T>(targetSize, ArrayPool<T>.Shared);

        if (!_channel.Reader.TryRead(out var firstItem))
        {
            lease.Dispose();
            return null;
        }

        lease.Buffer[0] = firstItem;
        Interlocked.Decrement(ref _queueDepth);
        lease.Count = 1;

        var waitStopwatch = Stopwatch.StartNew();

        while (lease.Count < targetSize)
        {
            while (lease.Count < targetSize && _channel.Reader.TryRead(out var nextItem))
            {
                lease.Buffer[lease.Count] = nextItem;
                lease.Count++;
                Interlocked.Decrement(ref _queueDepth);
            }

            if (lease.Count >= targetSize)
            {
                break;
            }

            var remaining = _options.AdaptiveBatching.MaxDelay - waitStopwatch.Elapsed;
            if (remaining <= TimeSpan.Zero)
            {
                break;
            }

            if (!await WaitToReadWithinAsync(remaining, ct).ConfigureAwait(false))
            {
                break;
            }
        }

        sizer.Observe(lease.Count, waitStopwatch.Elapsed);
        return lease;
    }

    private async Task<bool> WaitToReadWithinAsync(
        TimeSpan timeout,
        CancellationToken ct
    )
    {
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(timeout);

        try
        {
            return await _channel.Reader.WaitToReadAsync(timeoutCts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            return _channel.Reader.TryPeek(out _);
        }
    }
}
