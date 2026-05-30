using Microsoft.EntityFrameworkCore;
using SharedKernel.BulkInsertPipeline.Abstractions;
using SharedKernel.BulkInsertPipeline.Extensions;
using SharedKernel.BulkInsertPipeline.Test.Service.Configuration;
using SharedKernel.BulkInsertPipeline.Test.Service.Data;
using SharedKernel.BulkInsertPipeline.Test.Service.Domain;

namespace SharedKernel.BulkInsertPipeline.Test.Service.Services;

public sealed class PipelineDirectInsertRunner(
    IConfiguration configuration,
    BulkInsertPipelineLabOptions labOptions
)
{
    private readonly string _connectionString =
        configuration.GetConnectionString(BulkInsertPipelineLabDatabase.ConnectionStringName)
        ?? throw new InvalidOperationException(
            $"Connection string '{BulkInsertPipelineLabDatabase.ConnectionStringName}' was not found."
        );

    private readonly BulkInsertPipelineLabOptions _labOptions = labOptions;

    public async Task<int> InsertAsync(
        BulkInsertProviderType provider,
        ReadOnlyMemory<MetricReading> batch,
        CancellationToken cancellationToken = default
    )
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContextFactory<BulkInsertPipelineLabDbContext>(
            options => options.UseNpgsql(_connectionString)
        );
        services.AddBulkInsert(
                options => MetricReadingRegistration.ConfigureOptions(
                    options,
                    provider,
                    _labOptions.FallbackProvider,
                    1,
                    Math.Max(batch.Length, 16)
                )
            )
            .AddEntity<MetricReading>(
                entity => MetricReadingRegistration.ConfigureEntity(entity, _connectionString)
            );

        await using var serviceProvider = services.BuildServiceProvider(true);
        var bulkInsertService = serviceProvider.GetRequiredService<IBulkInsertService<MetricReading>>();

        await bulkInsertService.InsertAsync(batch, cancellationToken).ConfigureAwait(false);
        return batch.Length;
    }
}
