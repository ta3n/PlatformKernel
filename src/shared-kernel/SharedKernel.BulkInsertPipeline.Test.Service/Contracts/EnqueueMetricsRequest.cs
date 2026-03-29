namespace SharedKernel.BulkInsertPipeline.Test.Service.Contracts;

public sealed record EnqueueMetricsRequest(
    int Count = 100,
    string DevicePrefix = "sensor",
    string Source = "pipeline",
    int? WaitForDrainMilliseconds = null
);
