using NpgsqlTypes;
using SharedKernel.BulkInsertPipeline.Abstractions;
using SharedKernel.BulkInsertPipeline.Extensions;
using SharedKernel.BulkInsertPipeline.Options;
using SharedKernel.BulkInsertPipeline.Test.Service.Data;
using SharedKernel.BulkInsertPipeline.Test.Service.Domain;

namespace SharedKernel.BulkInsertPipeline.Test.Service.Services;

internal static class MetricReadingRegistration
{
    public static void ConfigureOptions(
        BulkInsertOptions options,
        BulkInsertProviderType primaryProvider,
        BulkInsertProviderType fallbackProvider,
        int workerCount,
        int channelCapacity
    )
    {
        ArgumentNullException.ThrowIfNull(options);

        options.WorkerCount = workerCount;
        options.ChannelCapacity = channelCapacity;
        options.Resilience.FallbackProvider = fallbackProvider;
        options.AdaptiveBatching.MinBatchSize = 16;
        options.AdaptiveBatching.MaxBatchSize = 256;
        options.AdaptiveBatching.MaxDelay = TimeSpan.FromMilliseconds(50);

        switch (primaryProvider)
        {
            case BulkInsertProviderType.NpgsqlBinaryCopy:
                options.UseNpgsqlCopy();
                break;
            case BulkInsertProviderType.Dapper:
                options.UseDapper();
                break;
            case BulkInsertProviderType.RepoDb:
                options.UseRepoDb();
                break;
            case BulkInsertProviderType.EfCore:
                options.UseEfCore();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(primaryProvider), primaryProvider, null);
        }
    }

    public static void ConfigureEntity(
        BulkInsertEntityBuilder<MetricReading> entity,
        string connectionString
    )
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        entity
            .ToTable(BulkInsertPipelineLabDatabase.Table, BulkInsertPipelineLabDatabase.Schema)
            .UseDirectConnectionString(connectionString)
            .UseOltpConnectionString(connectionString)
            .UseDbContextFactory<BulkInsertPipelineLabDbContext>()
            .MapColumn(static reading => reading.DeviceId, "device_id", NpgsqlDbType.Text)
            .MapColumn(static reading => reading.OccurredAt, "occurred_at", NpgsqlDbType.TimestampTz)
            .MapColumn(static reading => reading.Value, "value", NpgsqlDbType.Double)
            .MapColumn(static reading => reading.Quality, "quality", NpgsqlDbType.Integer, isNullable: true)
            .MapColumn(static reading => reading.Source, "source", NpgsqlDbType.Text);
    }
}
