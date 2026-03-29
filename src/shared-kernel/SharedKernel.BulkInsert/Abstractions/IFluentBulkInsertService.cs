using Microsoft.EntityFrameworkCore;
using SharedKernel.BulkInsert.Models;

namespace SharedKernel.BulkInsert.Abstractions;

public interface IFluentBulkInsertService<in TEntity>
    where TEntity : class
{
    Task<int> BulkInsertAsync(
        IEnumerable<TEntity> entities,
        PostgreSqlBulkInsertProvider? provider = null,
        PostgreSqlBulkInsertOptions? options = null,
        CancellationToken cancellationToken = default
    );

    Task<int> BulkInsertAsync(
        DbContext dbContext,
        IEnumerable<TEntity> entities,
        PostgreSqlBulkInsertProvider? provider = null,
        PostgreSqlBulkInsertOptions? options = null,
        CancellationToken cancellationToken = default
    );
}
