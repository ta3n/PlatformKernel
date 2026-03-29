using System.Diagnostics;
using System.Diagnostics.Metrics;
using SharedKernel.BulkInsertPipeline.Options;

namespace SharedKernel.BulkInsertPipeline.Observability;

internal sealed class BulkInsertTelemetry : IDisposable
{
    private readonly Meter _meter = new("SharedKernel.BulkInsert", "1.0.0");
    private readonly ActivitySource _activitySource = new("SharedKernel.BulkInsert");
    private readonly BulkInsertObservabilityOptions _options;
    private readonly Counter<long> _insertedRecords;
    private readonly Counter<long> _failedRecords;
    private readonly Counter<long> _rejectedRecords;
    private readonly Histogram<long> _batchSize;
    private readonly Histogram<double> _insertDurationMs;
    private readonly Histogram<double> _throughputRecordsPerSecond;

    public BulkInsertTelemetry(
        BulkInsertOptions options
    )
    {
        _options = options.Observability;
        _insertedRecords = _meter.CreateCounter<long>("bulk_insert.records_inserted");
        _failedRecords = _meter.CreateCounter<long>("bulk_insert.records_failed");
        _rejectedRecords = _meter.CreateCounter<long>("bulk_insert.records_rejected");
        _batchSize = _meter.CreateHistogram<long>("bulk_insert.batch_size", unit: "records");
        _insertDurationMs = _meter.CreateHistogram<double>("bulk_insert.insert_duration", unit: "ms");
        _throughputRecordsPerSecond = _meter.CreateHistogram<double>("bulk_insert.throughput", unit: "records/s");
    }

    public Activity? StartInsertActivity(
        string entityName,
        BulkInsertProviderType providerType,
        int batchSize,
        BulkInsertTableIdentifier target
    )
    {
        if (!_options.EnableTracing)
        {
            return null;
        }

        var activity = _activitySource.StartActivity("bulk-insert.execute", ActivityKind.Client);
        if (activity is null)
        {
            return null;
        }

        activity.SetTag("bulk.entity", entityName);
        activity.SetTag("bulk.provider", providerType.ToString());
        activity.SetTag("bulk.batch_size", batchSize);
        activity.SetTag("db.system", "postgresql");
        activity.SetTag("db.collection.name", target.Table);
        activity.SetTag("db.namespace", target.Schema);
        activity.SetTag("db.operation.name", "COPY");

        return activity;
    }

    public void RecordBatchSucceeded(
        string entityName,
        BulkInsertProviderType providerType,
        int batchSize,
        TimeSpan duration
    )
    {
        if (!_options.EnableMetrics)
        {
            return;
        }

        var tags = CreateTags(entityName, providerType);
        _batchSize.Record(batchSize, tags);
        _insertDurationMs.Record(duration.TotalMilliseconds, tags);
        _insertedRecords.Add(batchSize, tags);

        var throughput = duration.TotalSeconds <= 0d
            ? batchSize
            : batchSize / duration.TotalSeconds;

        _throughputRecordsPerSecond.Record(throughput, tags);
    }

    public void RecordBatchFailure(
        string entityName,
        BulkInsertProviderType providerType,
        int batchSize
    )
    {
        if (!_options.EnableMetrics)
        {
            return;
        }

        _failedRecords.Add(batchSize, CreateTags(entityName, providerType));
    }

    public void RecordRejected(
        string entityName,
        BulkInsertRejectionReason reason
    )
    {
        if (!_options.EnableMetrics)
        {
            return;
        }

        TagList tags = new()
        {
            { "bulk.entity", entityName },
            { "bulk.rejection_reason", reason.ToString() }
        };

        _rejectedRecords.Add(1, tags);
    }

    public void Dispose()
    {
        _activitySource.Dispose();
        _meter.Dispose();
    }

    private static TagList CreateTags(
        string entityName,
        BulkInsertProviderType providerType
    )
    {
        TagList tags = new()
        {
            { "bulk.entity", entityName },
            { "bulk.provider", providerType.ToString() }
        };

        return tags;
    }
}
