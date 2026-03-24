namespace PlatformKernel.TimescaleDbDemo;

public static class SeedDataFactory
{
    public static IReadOnlyList<MetricPoint> CreateSampleMetrics()
    {
        var baseline = new DateTimeOffset(2026, 03, 01, 00, 00, 00, TimeSpan.Zero);

        return
        [
            CreateMetric(baseline.AddMinutes(1), "device-01", 0.32, 0.61, 29.0, "sg", "r1"),
            CreateMetric(baseline.AddMinutes(4), "device-01", 0.41, 0.64, 29.2, "sg", "r1"),
            CreateMetric(baseline.AddMinutes(7), "device-02", 0.76, 0.83, 35.1, "hn", "r2"),
            CreateMetric(baseline.AddMinutes(9), "device-01", 0.28, 0.58, 28.8, "sg", "r1"),
            CreateMetric(baseline.AddHours(1).AddMinutes(3), "device-02", 0.80, 0.85, 35.4, "hn", "r2"),
            CreateMetric(baseline.AddHours(1).AddMinutes(5), "device-01", 0.45, 0.68, 30.0, "sg", "r1"),
            CreateMetric(baseline.AddHours(1).AddMinutes(10), "device-02", 0.70, 0.81, 34.8, "hn", "r2"),
            CreateMetric(baseline.AddHours(1).AddMinutes(12), "device-01", 0.49, 0.72, 30.3, "sg", "r1")
        ];
    }

    private static MetricPoint CreateMetric(
        DateTimeOffset timestamp,
        string deviceId,
        double cpu,
        double memory,
        double temperature,
        string site,
        string rack
    )
    {
        return new MetricPoint
        {
            Time = timestamp,
            DeviceId = deviceId,
            Cpu = cpu,
            Memory = memory,
            Temperature = temperature,
            Tags = $$"""{"site":"{{site}}","rack":"{{rack}}"}"""
        };
    }
}
