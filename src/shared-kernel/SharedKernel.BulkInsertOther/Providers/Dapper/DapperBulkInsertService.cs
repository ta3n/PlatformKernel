using Dapper;
using Microsoft.Extensions.Logging;
using SharedKernel.BulkInsertOther.Mapping;
using SharedKernel.BulkInsertOther.Options;
using SharedKernel.BulkInsertOther.Providers.Npgsql;

namespace SharedKernel.BulkInsertOther.Providers.Dapper;

public sealed class DapperBulkInsertService<T>(
    BulkInsertEntityDescriptor<T> descriptor,
    NpgsqlBulkInsertConnectionFactory<T> connectionFactory,
    BulkInsertOptions options,
    ILogger<DapperBulkInsertService<T>> logger
)
    : IBulkInsertProvider<T>
{
    public BulkInsertProviderType ProviderType => BulkInsertProviderType.Dapper;

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

        var maxRows = context.Descriptor.GetMaxRowsForParameterLimit(options.MaxFallbackCommandParameters);
        await using var connection = await connectionFactory.OpenConnectionAsync(ct).ConfigureAwait(false);

        foreach (var chunk in SqlFallbackBatchFactory.Chunk(batch, maxRows))
        {
            var commandText = context.Descriptor.BuildInsertSql(chunk.Length, context.Target);
            var parameters = SqlFallbackBatchFactory.CreateParameters(context.Descriptor, chunk);

            await connection.ExecuteAsync(
                    new CommandDefinition(commandText, parameters, cancellationToken: ct)
                )
                .ConfigureAwait(false);
        }

        if (options.Observability.EnableDetailedSuccessLogs)
        {
            logger.LogInformation(
                "Dapper fallback inserted {RowCount} rows for entity {EntityName} into {TargetTable}.",
                batch.Length,
                descriptor.EntityName,
                context.Target.QualifiedName
            );
        }
    }
}
