using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.BulkInsertPipeline.Mapping;
using SharedKernel.BulkInsertPipeline.Options;

namespace SharedKernel.BulkInsertPipeline.Providers.EFCore;

public sealed class EfCoreBulkInsertService<T>(
    BulkInsertEntityDescriptor<T> descriptor,
    BulkInsertOptions options,
    IServiceProvider serviceProvider,
    ILogger<EfCoreBulkInsertService<T>> logger
)
    : IBulkInsertProvider<T>
{
    public BulkInsertProviderType ProviderType => BulkInsertProviderType.EfCore;

    public bool IsFallbackOnly => true;

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

        if (descriptor.DbContextFactory is null)
        {
            throw new InvalidOperationException(
                $"Entity '{typeof(T).FullName}' is not configured with a DbContext factory. Register one via AddEntity(...).UseDbContextFactory<TDbContext>() before using the EF Core fallback provider."
            );
        }

        var maxRows = context.Descriptor.GetMaxRowsForParameterLimit(options.MaxFallbackCommandParameters);

        await using var dbContext = descriptor.DbContextFactory.Invoke(serviceProvider);
        dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

        foreach (var chunk in SqlFallbackBatchFactory.Chunk(batch, maxRows))
        {
            var sql = context.Descriptor.BuildRawInsertSql(chunk.Length, context.Target);
            var values = SqlFallbackBatchFactory.ExtractValues(context.Descriptor, chunk);
            await dbContext.Database.ExecuteSqlRawAsync(
                    sql,
                    values.Select(static value => value ?? DBNull.Value),
                    ct
                )
                .ConfigureAwait(false);
        }

        if (options.Observability.EnableDetailedSuccessLogs)
        {
            logger.LogInformation(
                "EF Core fallback inserted {RowCount} rows for entity {EntityName} into {TargetTable} without using tracked entities.",
                batch.Length,
                descriptor.EntityName,
                context.Target.QualifiedName
            );
        }
    }
}
