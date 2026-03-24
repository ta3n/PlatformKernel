namespace PlatformKernel.TimescaleDbDemo.Tests;

public sealed class TimescaleDemoRunnerTests(
    TimescaleContainerFixture fixture
) : IClassFixture<TimescaleContainerFixture>
{
    [Fact]
    public async Task BootstrapAsync_CreatesSchemaAndBackgroundPolicies()
    {
        var runner = new TimescaleDemoRunner(await fixture.CreateIsolatedConnectionStringAsync());

        await runner.BootstrapAsync();

        var summary = await runner.GetSummaryAsync();

        Assert.True(summary.ModernHypertableExists);
        Assert.Contains("metrics", summary.Hypertables);
        Assert.True(summary.HasContinuousAggregatePolicy);
        Assert.True(summary.HasRetentionPolicy);
        Assert.True(summary.HasColumnstorePolicy);
    }

    [Fact]
    public async Task SeedSampleDataAsync_AndRefreshContinuousAggregate_ReturnExpectedData()
    {
        var runner = new TimescaleDemoRunner(await fixture.CreateIsolatedConnectionStringAsync());

        await runner.BootstrapAsync();
        var seededRows = await runner.SeedSampleDataAsync();
        await runner.RefreshContinuousAggregateAsync();

        var summary = await runner.GetSummaryAsync();

        Assert.Equal(seededRows.Count, summary.SeededRowCount);
        Assert.NotEmpty(summary.BucketedMetrics);
        Assert.NotEmpty(summary.HourlyAggregates);
        Assert.Equal(seededRows.Count, summary.BucketedMetrics.Sum(metric => metric.RowCount));
        Assert.True(summary.ChunkCount >= 1);
    }

    [Fact]
    public async Task RunLegacyMigrationDemoAsync_ConvertsPlainTableToHypertable()
    {
        var runner = new TimescaleDemoRunner(await fixture.CreateIsolatedConnectionStringAsync());

        await runner.BootstrapAsync();
        var result = await runner.RunLegacyMigrationDemoAsync();

        Assert.True(result.IsHypertable);
        Assert.Equal(3, result.RowCount);
    }

    [Fact]
    public async Task ProbeTieringAsync_ReturnsCapabilityInformationWithoutThrowing()
    {
        var runner = new TimescaleDemoRunner(await fixture.CreateIsolatedConnectionStringAsync());

        await runner.BootstrapAsync();
        var result = await runner.ProbeTieringAsync();

        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }
}
