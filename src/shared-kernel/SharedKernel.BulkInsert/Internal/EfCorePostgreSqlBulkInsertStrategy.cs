using Microsoft.EntityFrameworkCore;
using SharedKernel.BulkInsert.Models;

namespace SharedKernel.BulkInsert.Internal;

internal sealed class EfCorePostgreSqlBulkInsertStrategy : IPostgreSqlBulkInsertStrategy
{
    public PostgreSqlBulkInsertProvider Provider => PostgreSqlBulkInsertProvider.EfCore;

    public async Task<int> BulkInsertAsync<TEntity>(
        DbContext dbContext,
        IReadOnlyCollection<TEntity> entities,
        PostgreSqlBulkInsertMetadata<TEntity> metadata,
        PostgreSqlBulkInsertOptions options,
        CancellationToken cancellationToken
    )
        where TEntity : class
    {
        if (entities.Count == 0)
        {
            return 0;
        }

        var batchSize = PostgreSqlBulkInsertSqlHelper.ResolveBatchSize(
            options.BatchSize,
            entities.Count,
            metadata.Columns.Count
        );
        var originalAutoDetectChanges = dbContext.ChangeTracker.AutoDetectChangesEnabled;
        var ownsTransaction = dbContext.Database.CurrentTransaction is null;

        await using var transaction = ownsTransaction
            ? await dbContext.Database.BeginTransactionAsync(cancellationToken)
            : null;

        try
        {
            dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

            var affectedRows = 0;
            foreach (var batch in entities.Chunk(batchSize))
            {
                await dbContext.Set<TEntity>().AddRangeAsync(batch, cancellationToken);
                affectedRows += await dbContext.SaveChangesAsync(cancellationToken);

                foreach (var entity in batch)
                {
                    dbContext.Entry(entity).State = EntityState.Detached;
                }
            }

            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return affectedRows;
        }
        catch
        {
            if (transaction is not null)
            {
                await transaction.RollbackAsync(cancellationToken);
            }

            throw;
        }
        finally
        {
            dbContext.ChangeTracker.AutoDetectChangesEnabled = originalAutoDetectChanges;
        }
    }
}
