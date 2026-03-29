using Microsoft.EntityFrameworkCore;
using SharedKernel.BulkInsert.Models;

namespace SharedKernel.BulkInsert.Abstractions;

public interface IPostgreSqlBulkInsertService
{
    Task<int> BulkInsertAsync<TEntity>(
        DbContext dbContext,
        IEnumerable<TEntity> entities,
        PostgreSqlBulkInsertProvider provider = PostgreSqlBulkInsertProvider.RepoDb,
        PostgreSqlBulkInsertOptions? options = null,
        CancellationToken cancellationToken = default
    )
        where TEntity : class;
}
