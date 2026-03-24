namespace PlatformKernel.TimescaleDbDemo;

public sealed class MetricPoint
{
    public DateTimeOffset Time { get; set; }

    public string DeviceId { get; set; } = string.Empty;

    public double Cpu { get; set; }

    public double Memory { get; set; }

    public double Temperature { get; set; }

    public string? Tags { get; set; }
}

public sealed class BucketedMetric
{
    public DateTimeOffset Bucket { get; set; }

    public string DeviceId { get; set; } = string.Empty;

    public double AvgCpu { get; set; }

    public double MaxTemperature { get; set; }

    public long RowCount { get; set; }
}

public sealed class HourlyMetricAggregate
{
    public DateTimeOffset Bucket { get; set; }

    public string DeviceId { get; set; } = string.Empty;

    public double AvgCpu { get; set; }

    public double MaxCpu { get; set; }

    public double MinCpu { get; set; }

    public long TotalRows { get; set; }
}

public sealed class TimescaleJobInfo
{
    public int JobId { get; set; }

    public string ProcName { get; set; } = string.Empty;
}

public sealed record TieringAttemptResult(
    bool FunctionAvailable,
    bool PolicyApplied,
    string Message);

public sealed record LegacyMigrationResult(
    bool IsHypertable,
    int RowCount);

public sealed record TimescaleSummary(
    string TimescaleVersion,
    bool ToolkitInstalled,
    bool ModernHypertableExists,
    int SeededRowCount,
    int ChunkCount,
    bool HasContinuousAggregatePolicy,
    bool HasRetentionPolicy,
    bool HasColumnstorePolicy,
    IReadOnlyList<string> Hypertables,
    IReadOnlyList<BucketedMetric> BucketedMetrics,
    IReadOnlyList<HourlyMetricAggregate> HourlyAggregates,
    IReadOnlyList<TimescaleJobInfo> Jobs);

public sealed record DemoExecutionResult(
    TimescaleSummary Summary,
    LegacyMigrationResult LegacyMigration,
    TieringAttemptResult Tiering);
