using System.Collections.Concurrent;
using Microsoft.Extensions.Logging.Abstractions;
using SharedKernel.BulkInsert.Abstractions;
using SharedKernel.BulkInsert.Mapping;
using SharedKernel.BulkInsert.Observability;
using SharedKernel.BulkInsert.Options;
using SharedKernel.BulkInsert.Pipeline;
using SharedKernel.BulkInsert.Test.TestEntities;

namespace SharedKernel.BulkInsert.Test.Unit;

public sealed class BulkInsertPipelineTests
{
    [Fact]
    public async Task Pipeline_drains_buffered_items_during_stop()
    {
        var options = CreateOptions();
        var service = new RecordingBulkInsertService<MetricPoint>();
        var sink = new RecordingFailureSink<MetricPoint>();
        var descriptor = CreateDescriptor();
        var telemetry = new BulkInsertTelemetry(options);
        var pipeline = new BulkInsertPipeline<MetricPoint>(
            options,
            service,
            sink,
            telemetry,
            descriptor,
            NullLogger<BulkInsertPipeline<MetricPoint>>.Instance
        );

        await pipeline.StartAsync(CancellationToken.None);

        for (var index = 0; index < 5; index++)
        {
            await pipeline.EnqueueAsync(
                new MetricPoint($"sensor-{index}", DateTimeOffset.UtcNow, index, index),
                CancellationToken.None
            );
        }

        await pipeline.StopAsync(CancellationToken.None);

        Assert.Equal(5, service.Inserted.Count);
        Assert.Empty(sink.Rejections);
    }

    [Fact]
    public async Task Pipeline_parks_items_when_channel_is_over_capacity()
    {
        var options = CreateOptions();
        options.EnableBackpressure = false;
        options.OverloadStrategy = BulkInsertOverloadStrategy.Park;
        options.ChannelCapacity = 1;
        options.AdaptiveBatching.MinBatchSize = 1;
        options.AdaptiveBatching.MaxBatchSize = 1;

        var service = new BlockingBulkInsertService<MetricPoint>();
        var sink = new RecordingFailureSink<MetricPoint>();
        var descriptor = CreateDescriptor();
        var telemetry = new BulkInsertTelemetry(options);
        var pipeline = new BulkInsertPipeline<MetricPoint>(
            options,
            service,
            sink,
            telemetry,
            descriptor,
            NullLogger<BulkInsertPipeline<MetricPoint>>.Instance
        );

        await pipeline.StartAsync(CancellationToken.None);

        await pipeline.EnqueueAsync(new MetricPoint("sensor-1", DateTimeOffset.UtcNow, 1d, 1), CancellationToken.None);
        await service.FirstInsertStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));

        for (var index = 2; index <= 12; index++)
        {
            await pipeline.EnqueueAsync(
                new MetricPoint($"sensor-{index}", DateTimeOffset.UtcNow, index, index),
                CancellationToken.None
            );
        }

        service.Release();
        await pipeline.StopAsync(CancellationToken.None);

        Assert.Contains(
            sink.Rejections,
            rejection => rejection.Reason == BulkInsertRejectionReason.CapacityExceeded
        );
    }

    private static BulkInsertOptions CreateOptions()
    {
        return new BulkInsertOptions
        {
            WorkerCount = 1,
            ChannelCapacity = 8,
            ShutdownDrainTimeout = TimeSpan.FromSeconds(5)
        };
    }

    private static BulkInsertEntityDescriptor<MetricPoint> CreateDescriptor()
    {
        var entityOptions = new BulkInsertEntityOptions<MetricPoint>();
        entityOptions.MapColumn(static point => point.DeviceId, "DeviceId", NpgsqlTypes.NpgsqlDbType.Text);
        entityOptions.MapColumn(static point => point.OccurredAt, "OccurredAt", NpgsqlTypes.NpgsqlDbType.TimestampTz);
        entityOptions.MapColumn(static point => point.Value, "Value", NpgsqlTypes.NpgsqlDbType.Double);
        entityOptions.MapColumn(static point => point.Quality, "Quality", NpgsqlTypes.NpgsqlDbType.Integer, isNullable: true);

        return new BulkInsertEntityDescriptor<MetricPoint>(entityOptions);
    }

    private sealed class RecordingBulkInsertService<T> : IBulkInsertService<T>
    {
        public ConcurrentQueue<T> Inserted { get; } = new();

        public Task InsertAsync(ReadOnlyMemory<T> batch, CancellationToken ct = default)
        {
            foreach (var item in batch.Span)
            {
                Inserted.Enqueue(item);
            }

            return Task.CompletedTask;
        }
    }

    private sealed class BlockingBulkInsertService<T> : IBulkInsertService<T>
    {
        private readonly TaskCompletionSource _release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource FirstInsertStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public async Task InsertAsync(ReadOnlyMemory<T> batch, CancellationToken ct = default)
        {
            FirstInsertStarted.TrySetResult();
            await _release.Task.WaitAsync(ct);
        }

        public void Release() => _release.TrySetResult();
    }

    private sealed class RecordingFailureSink<T> : IBulkInsertFailureSink<T>
    {
        public ConcurrentQueue<(T Item, BulkInsertRejectionReason Reason)> Rejections { get; } = new();

        public ConcurrentQueue<(ReadOnlyMemory<T> Batch, Exception Exception)> FailedBatches { get; } = new();

        public ValueTask OnRejectedAsync(
            T item,
            BulkInsertRejectionReason reason,
            CancellationToken ct = default
        )
        {
            Rejections.Enqueue((item, reason));
            return ValueTask.CompletedTask;
        }

        public ValueTask OnBatchFailedAsync(
            ReadOnlyMemory<T> batch,
            Exception exception,
            CancellationToken ct = default
        )
        {
            FailedBatches.Enqueue((batch, exception));
            return ValueTask.CompletedTask;
        }
    }
}
