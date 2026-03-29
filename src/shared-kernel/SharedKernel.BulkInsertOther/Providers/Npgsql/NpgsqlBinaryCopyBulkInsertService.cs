using Microsoft.Extensions.Logging;
using SharedKernel.BulkInsertOther.Mapping;
using SharedKernel.BulkInsertOther.Options;

namespace SharedKernel.BulkInsertOther.Providers.Npgsql;

public sealed class NpgsqlBinaryCopyBulkInsertService<T>(
    BulkInsertEntityDescriptor<T> descriptor,
    NpgsqlBulkInsertConnectionFactory<T> connectionFactory,
    BulkInsertOptions options,
    ILogger<NpgsqlBinaryCopyBulkInsertService<T>> logger
)
    : IBulkInsertProvider<T>
{
    public BulkInsertProviderType ProviderType => BulkInsertProviderType.NpgsqlBinaryCopy;

    public bool IsFallbackOnly => false;

    public Task InsertAsync(
        ReadOnlyMemory<T> batch,
        CancellationToken ct = default
    )
    {
        return InsertCoreAsync(batch, new BulkInsertExecutionContext<T>(descriptor, descriptor.Table), ct);
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

        if (string.IsNullOrWhiteSpace(descriptor.Connection.DirectConnectionString))
        {
            throw new InvalidOperationException(
                $"Entity '{typeof(T).FullName}' must be configured with a direct PostgreSQL connection string for COPY. Route OLTP traffic through PgBouncer if needed, but bypass it for bulk COPY."
            );
        }

        await using var connection = await connectionFactory.OpenDirectConnectionAsync(ct).ConfigureAwait(false);
        await using var importer = await connection.BeginBinaryImportAsync(
                context.Descriptor.BuildCopyCommand(context.Target),
                ct
            )
            .ConfigureAwait(false);

        importer.Timeout = descriptor.Connection.ImportTimeout;

        for (var index = 0; index < batch.Length; index++)
        {
            var item = batch.Span[index];
            await context.Descriptor.Mapper.WriteAsync(item, importer, ct).ConfigureAwait(false);
        }

        await importer.CompleteAsync(ct).ConfigureAwait(false);

        if (options.Observability.EnableDetailedSuccessLogs)
        {
            logger.LogInformation(
                "Binary COPY inserted {RowCount} rows for entity {EntityName} into {TargetTable}.",
                batch.Length,
                descriptor.EntityName,
                context.Target.QualifiedName
            );
        }
    }
}
