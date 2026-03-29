using System.Globalization;
using SharedKernel.BulkInsertPipeline.Test.Service.Domain;

namespace SharedKernel.BulkInsertPipeline.Test.Service.Services;

internal static class MetricReadingFactory
{
    public static MetricReading[] CreateBatch(
        int count,
        string devicePrefix,
        string source
    )
    {
        var normalizedPrefix = string.IsNullOrWhiteSpace(devicePrefix) ? "sensor" : devicePrefix.Trim();
        var normalizedSource = string.IsNullOrWhiteSpace(source) ? "mini-api" : source.Trim();
        var batchToken = Guid.NewGuid().ToString("N");
        var occurredAt = DateTimeOffset.UtcNow;
        var batch = new MetricReading[count];

        for (var index = 0; index < count; index++)
        {
            batch[index] = new MetricReading
            {
                DeviceId = string.Concat(
                    normalizedPrefix,
                    "-",
                    batchToken,
                    "-",
                    index.ToString("D4", CultureInfo.InvariantCulture)
                ),
                OccurredAt = occurredAt.AddSeconds(index),
                Value = index * 1.25d,
                Quality = index % 5,
                Source = normalizedSource
            };
        }

        return batch;
    }
}
