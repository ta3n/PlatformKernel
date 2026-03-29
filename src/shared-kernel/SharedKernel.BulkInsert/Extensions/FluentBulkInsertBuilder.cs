using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharedKernel.BulkInsert.Abstractions;
using SharedKernel.BulkInsert.Configuration;
using SharedKernel.BulkInsert.Models;
using SharedKernel.BulkInsert.Services;

namespace SharedKernel.BulkInsert.Extensions;

public sealed class FluentBulkInsertBuilder(
    IServiceCollection services
)
{
    public IServiceCollection Services { get; } = services;

    public FluentBulkInsertBuilder AddEntity<TEntity>(
        Action<FluentBulkInsertEntityBuilder<TEntity>>? configure = null
    )
        where TEntity : class
    {
        var options = new FluentBulkInsertEntityOptions<TEntity>();
        configure?.Invoke(new FluentBulkInsertEntityBuilder<TEntity>(options));

        Services.RemoveAll<FluentBulkInsertEntityOptions<TEntity>>();
        Services.RemoveAll<IFluentBulkInsertService<TEntity>>();

        Services.AddSingleton(options);
        Services.AddTransient<IFluentBulkInsertService<TEntity>, FluentBulkInsertService<TEntity>>();

        return this;
    }
}

public sealed class FluentBulkInsertEntityBuilder<TEntity>(
    FluentBulkInsertEntityOptions<TEntity> options
)
    where TEntity : class
{
    public FluentBulkInsertEntityBuilder<TEntity> UseProvider(
        PostgreSqlBulkInsertProvider provider
    )
    {
        options.Provider = provider;
        return this;
    }

    public FluentBulkInsertEntityBuilder<TEntity> UseRepoDb()
    {
        options.Provider = PostgreSqlBulkInsertProvider.RepoDb;
        return this;
    }

    public FluentBulkInsertEntityBuilder<TEntity> UseDapper()
    {
        options.Provider = PostgreSqlBulkInsertProvider.Dapper;
        return this;
    }

    public FluentBulkInsertEntityBuilder<TEntity> UseEfCore()
    {
        options.Provider = PostgreSqlBulkInsertProvider.EfCore;
        return this;
    }

    public FluentBulkInsertEntityBuilder<TEntity> WithBatchSize(
        int batchSize
    )
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(batchSize);
        options.BatchSize = batchSize;
        return this;
    }

    public FluentBulkInsertEntityBuilder<TEntity> WithTimeoutSeconds(
        int timeoutSeconds
    )
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeoutSeconds);
        options.TimeoutSeconds = timeoutSeconds;
        return this;
    }

    public FluentBulkInsertEntityBuilder<TEntity> UseDbContextFactory<TDbContext>()
        where TDbContext : DbContext
    {
        options.DbContextFactory = static serviceProvider =>
            serviceProvider.GetRequiredService<IDbContextFactory<TDbContext>>().CreateDbContext();

        return this;
    }

    public FluentBulkInsertEntityBuilder<TEntity> UseDbContextFactory(
        Func<IServiceProvider, DbContext> dbContextFactory
    )
    {
        options.DbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        return this;
    }
}
