using Microsoft.EntityFrameworkCore;
using SharedKernel.BulkInsert.Models;

namespace SharedKernel.BulkInsert.Internal;

internal interface IPostgreSqlBulkInsertStrategy
{
    PostgreSqlBulkInsertProvider Provider { get; }

    Task<int> BulkInsertAsync<TEntity>(
        DbContext dbContext,
        IReadOnlyCollection<TEntity> entities,
        PostgreSqlBulkInsertMetadata<TEntity> metadata,
        PostgreSqlBulkInsertOptions options,
        CancellationToken cancellationToken
    )
        where TEntity : class;
}
