using Microsoft.Extensions.Logging;
using RepoDb;
using SharedKernel.BulkInsertPipeline.Mapping;
using SharedKernel.BulkInsertPipeline.Options;
using SharedKernel.BulkInsertPipeline.Providers.Npgsql;

namespace SharedKernel.BulkInsertPipeline.Providers.RepoDb;

public sealed class RepoDbBulkInsertService<T> : IBulkInsertProvider<T>
    where T : class
{
    private readonly BulkInsertEntityDescriptor<T> _descriptor;
    private readonly NpgsqlBulkInsertConnectionFactory<T> _connectionFactory;
    private readonly BulkInsertOptions _options;
    private readonly ILogger<RepoDbBulkInsertService<T>> _logger;

    public RepoDbBulkInsertService(
        BulkInsertEntityDescriptor<T> descriptor,
        NpgsqlBulkInsertConnectionFactory<T> connectionFactory,
        BulkInsertOptions options,
        ILogger<RepoDbBulkInsertService<T>> logger
    )
    {
        _descriptor = descriptor;
        _connectionFactory = connectionFactory;
        _options = options;
        _logger = logger;

        GlobalConfiguration.Setup().UsePostgreSql();
    }

    public BulkInsertProviderType ProviderType => BulkInsertProviderType.RepoDb;

    public bool IsFallbackOnly => false;

    public Task InsertAsync(
        ReadOnlyMemory<T> batch,
        CancellationToken ct = default
    )
    {
        return InsertCoreAsync(batch, new BulkInsertExecutionContext<T>(_descriptor, _descriptor.Table), ct);
    }

    Task IBulkInsertProvider<T>.InsertAsync(
        ReadOnlyMemory<T> batch,
        BulkInsertExecutionContext<T> context,
        CancellationToken ct
    )
    {
        return InsertCoreAsync(batch, context, ct);
    }

    private async Task InsertCoreAsync(
        ReadOnlyMemory<T> batch,
        BulkInsertExecutionContext<T> context,
        CancellationToken ct = default
    )
    {
        if (batch.IsEmpty)
        {
            return;
        }

        await using var npgsqlConnection = await _connectionFactory.OpenConnectionAsync(ct).ConfigureAwait(false);

        await npgsqlConnection.BinaryBulkInsertAsync(
                context.Target.QualifiedName,
                batch.ToArray(),
                batchSize: batch.Length,
                cancellationToken: ct
            )
            .ConfigureAwait(false);

        if (_options.Observability.EnableDetailedSuccessLogs)
        {
            _logger.LogInformation(
                "RepoDb fallback inserted {RowCount} rows for entity {EntityName} into {TargetTable}.",
                batch.Length,
                _descriptor.EntityName,
                context.Target.QualifiedName
            );
        }
    }
}
