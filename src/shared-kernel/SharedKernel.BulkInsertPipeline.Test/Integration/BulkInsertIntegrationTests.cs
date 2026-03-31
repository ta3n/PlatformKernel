using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NpgsqlTypes;
using System.Globalization;
using SharedKernel.BulkInsertPipeline.Abstractions;
using SharedKernel.BulkInsertPipeline.Extensions;
using SharedKernel.BulkInsertPipeline.Options;
using SharedKernel.BulkInsertPipeline.Test.TestEntities;

namespace SharedKernel.BulkInsertPipeline.Test.Integration;

[Collection(BulkInsertPostgresCollection.Name)]
public sealed class BulkInsertIntegrationTests(
    BulkInsertPostgresFixture fixture
)
{
    [Fact]
    public async Task Npgsql_binary_copy_inserts_into_hypertable()
    {
        var connectionString = await fixture.CreateInitializedDatabaseAsync();
        await using var provider = BuildProvider(
            options => options.UseNpgsqlCopy(),
            builder => builder
                .ToHypertable("metric_buffer", "occurred_at")
                .UseDirectConnectionString(connectionString)
                .MapColumn(static point => point.DeviceId, "device_id", NpgsqlDbType.Text)
                .MapColumn(static point => point.OccurredAt, "occurred_at", NpgsqlDbType.TimestampTz)
                .MapColumn(static point => point.Value, "value", NpgsqlDbType.Double)
                .MapColumn(static point => point.Quality, "quality", NpgsqlDbType.Integer, true)
        );

        var service = provider.GetRequiredService<IBulkInsertService<MetricPoint>>();

        await service.InsertAsync(CreateBatch(32));

        Assert.Equal(32, await BulkInsertPostgresFixture.CountRowsAsync(connectionString, "metric_buffer"));
    }

    [Fact]
    public async Task Dapper_fallback_handles_missing_direct_copy_connection()
    {
        var connectionString = await fixture.CreateInitializedDatabaseAsync();
        await using var provider = BuildProvider(
            options =>
            {
                options.UseNpgsqlCopy();
                options.Resilience.FallbackProvider = BulkInsertProviderType.Dapper;
            },
            builder => builder
                .ToTable("MetricFallback")
                .UseOltpConnectionString(connectionString)
                .MapColumn(static point => point.DeviceId, "DeviceId", NpgsqlDbType.Text)
                .MapColumn(static point => point.OccurredAt, "OccurredAt", NpgsqlDbType.TimestampTz)
                .MapColumn(static point => point.Value, "Value", NpgsqlDbType.Double)
                .MapColumn(static point => point.Quality, "Quality", NpgsqlDbType.Integer, true)
        );

        var service = provider.GetRequiredService<IBulkInsertService<MetricPoint>>();

        await service.InsertAsync(CreateBatch(12));

        Assert.Equal(12, await BulkInsertPostgresFixture.CountRowsAsync(connectionString, "MetricFallback"));
    }

    [Fact]
    public async Task RepoDb_provider_inserts_batch_when_selected()
    {
        var connectionString = await fixture.CreateInitializedDatabaseAsync();
        await using var provider = BuildProvider(
            options => options.UseRepoDb(),
            builder => builder
                .ToTable("MetricFallback")
                .UseOltpConnectionString(connectionString)
                .MapColumn(static point => point.DeviceId, "DeviceId", NpgsqlDbType.Text)
                .MapColumn(static point => point.OccurredAt, "OccurredAt", NpgsqlDbType.TimestampTz)
                .MapColumn(static point => point.Value, "Value", NpgsqlDbType.Double)
                .MapColumn(static point => point.Quality, "Quality", NpgsqlDbType.Integer, true)
        );

        var service = provider.GetRequiredService<IBulkInsertService<MetricPoint>>();

        await service.InsertAsync(CreateBatch(10));

        Assert.Equal(10, await BulkInsertPostgresFixture.CountRowsAsync(connectionString, "MetricFallback"));
    }

    [Fact]
    public async Task EfCore_provider_executes_parameterized_sql_without_tracking()
    {
        var connectionString = await fixture.CreateInitializedDatabaseAsync();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContextFactory<TestMetricsDbContext>(options => options.UseNpgsql(connectionString));
        services.AddBulkInsert(options => options.UseEfCore())
            .AddEntity<MetricPoint>(
                builder => builder
                    .ToTable("MetricFallback")
                    .UseDbContextFactory<TestMetricsDbContext>()
                    .MapColumn(static point => point.DeviceId, "DeviceId", NpgsqlDbType.Text)
                    .MapColumn(static point => point.OccurredAt, "OccurredAt", NpgsqlDbType.TimestampTz)
                    .MapColumn(static point => point.Value, "Value", NpgsqlDbType.Double)
                    .MapColumn(static point => point.Quality, "Quality", NpgsqlDbType.Integer, true)
            );

        await using var provider = services.BuildServiceProvider(true);
        var service = provider.GetRequiredService<IBulkInsertService<MetricPoint>>();

        await service.InsertAsync(CreateBatch(9));

        Assert.Equal(9, await BulkInsertPostgresFixture.CountRowsAsync(connectionString, "MetricFallback"));
    }

    [Fact]
    public async Task Hosted_pipeline_drains_items_on_shutdown()
    {
        var connectionString = await fixture.CreateInitializedDatabaseAsync();

        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddLogging();
        builder.Services.AddBulkInsert(
                options =>
                {
                    options.UseNpgsqlCopy();
                    options.WorkerCount = 2;
                    options.ChannelCapacity = 64;
                    options.AdaptiveBatching.MinBatchSize = 4;
                    options.AdaptiveBatching.MaxBatchSize = 8;
                    options.AdaptiveBatching.MaxDelay = TimeSpan.FromMilliseconds(25);
                }
            )
            .AddEntity<MetricPoint>(
                entity => entity
                    .ToHypertable("metric_buffer", "occurred_at")
                    .UseDirectConnectionString(connectionString)
                    .MapColumn(static point => point.DeviceId, "device_id", NpgsqlDbType.Text)
                    .MapColumn(static point => point.OccurredAt, "occurred_at", NpgsqlDbType.TimestampTz)
                    .MapColumn(static point => point.Value, "value", NpgsqlDbType.Double)
                    .MapColumn(static point => point.Quality, "quality", NpgsqlDbType.Integer, true)
            );

        using var host = builder.Build();
        await host.StartAsync();

        var pipeline = host.Services.GetRequiredService<IBulkInsertPipeline<MetricPoint>>();
        var batch = CreateBatch(25).ToArray();
        for (var index = 0; index < batch.Length; index++)
        {
            await pipeline.EnqueueAsync(batch[index]);
        }

        await host.StopAsync();

        Assert.Equal(25, await BulkInsertPostgresFixture.CountRowsAsync(connectionString, "metric_buffer"));
    }

    private static ServiceProvider BuildProvider(
        Action<BulkInsertOptions> configureOptions,
        Action<BulkInsertEntityBuilder<MetricPoint>> configureEntity
    )
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddBulkInsert(configureOptions)
            .AddEntity(configureEntity);

        return services.BuildServiceProvider(true);
    }

    private static ReadOnlyMemory<MetricPoint> CreateBatch(
        int count
    )
    {
        var batch = new MetricPoint[count];
        var baseline = DateTimeOffset.Parse("2026-03-29T00:00:00+00:00", CultureInfo.InvariantCulture);

        for (var index = 0; index < batch.Length; index++)
        {
            batch[index] = new MetricPoint(
                $"sensor-{index:D4}",
                baseline.AddSeconds(index),
                index * 1.5d,
                index % 5
            );
        }

        return batch;
    }
}
