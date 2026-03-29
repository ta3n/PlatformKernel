using SharedKernel.BulkInsertPipeline.Abstractions;

namespace SharedKernel.BulkInsertPipeline.Test.Service.Contracts;

public sealed record InsertMetricsRequest(
    int Count = 100,
    string DevicePrefix = "sensor",
    string Source = "direct",
    BulkInsertProviderType Provider = BulkInsertProviderType.NpgsqlBinaryCopy
);
