using Microsoft.EntityFrameworkCore;

namespace SharedKernel.BulkInsert.Internal;

internal interface IPostgreSqlBulkInsertMetadataResolver
{
    PostgreSqlBulkInsertMetadata<TEntity> Resolve<TEntity>(
        DbContext dbContext
    )
        where TEntity : class;
}
