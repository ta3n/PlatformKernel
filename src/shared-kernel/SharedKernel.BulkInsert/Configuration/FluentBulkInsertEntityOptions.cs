using Microsoft.EntityFrameworkCore;
using SharedKernel.BulkInsert.Models;

namespace SharedKernel.BulkInsert.Configuration;

public sealed class FluentBulkInsertEntityOptions<TEntity>
    where TEntity : class
{
    public string EntityName { get; } = typeof(TEntity).Name;

    public PostgreSqlBulkInsertProvider Provider { get; set; } = PostgreSqlBulkInsertProvider.RepoDb;

    public int? BatchSize { get; set; }

    public int? TimeoutSeconds { get; set; }

    public Func<IServiceProvider, DbContext>? DbContextFactory { get; set; }
}
