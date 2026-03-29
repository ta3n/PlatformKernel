using NpgsqlTypes;
using SharedKernel.BulkInsert.Mapping;

namespace SharedKernel.BulkInsert.Test.TestEntities;

public sealed class AttributedMetricPoint
{
    [BulkInsertColumn("device_id", NpgsqlDbType.Text)]
    public string DeviceId { get; init; } = string.Empty;

    [BulkInsertColumn("occurred_at", NpgsqlDbType.TimestampTz)]
    public DateTimeOffset OccurredAt { get; init; }

    [BulkInsertColumn("value", NpgsqlDbType.Double)]
    public double Value { get; init; }

    [BulkInsertIgnore]
    public string IgnoredValue { get; init; } = string.Empty;
}
