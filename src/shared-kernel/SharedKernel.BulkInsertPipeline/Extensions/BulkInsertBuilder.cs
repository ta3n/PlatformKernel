using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NpgsqlTypes;
using SharedKernel.BulkInsertPipeline.Mapping;
using SharedKernel.BulkInsertPipeline.Pipeline;

namespace SharedKernel.BulkInsertPipeline.Extensions;

public sealed class BulkInsertBuilder(
    IServiceCollection services
)
{
    public IServiceCollection Services { get; } = services;

    public BulkInsertBuilder AddEntity<T>(
        Action<BulkInsertEntityBuilder<T>> configure
    )
    {
        var options = new BulkInsertEntityOptions<T>();
        configure(new BulkInsertEntityBuilder<T>(options));

        Services.AddSingleton(options);
        Services.AddSingleton<BulkInsertEntityDescriptor<T>>();
        Services.AddSingleton<BulkInsertPipeline<T>>();
        Services.AddSingleton<IBulkInsertPipeline<T>>(sp => sp.GetRequiredService<BulkInsertPipeline<T>>());
        Services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService>(
            sp => sp.GetRequiredService<BulkInsertPipeline<T>>()
        );

        return this;
    }
}

public sealed class BulkInsertEntityBuilder<T>(
    BulkInsertEntityOptions<T> options
)
{
    public BulkInsertEntityBuilder<T> ToTable(
        string table,
        string schema = "public"
    )
    {
        options.ToTable(table, schema);
        return this;
    }

    public BulkInsertEntityBuilder<T> ToHypertable(
        string table,
        string timeColumn,
        string schema = "public"
    )
    {
        options.ToHypertable(table, timeColumn, schema);
        return this;
    }

    public BulkInsertEntityBuilder<T> UseDirectConnectionString(
        string connectionString
    )
    {
        options.UseDirectConnectionString(connectionString);
        return this;
    }

    public BulkInsertEntityBuilder<T> UseOltpConnectionString(
        string connectionString
    )
    {
        options.UseOltpConnectionString(connectionString);
        return this;
    }

    public BulkInsertEntityBuilder<T> UsePartitionRouter(
        IBulkInsertPartitionRouter<T> partitionRouter
    )
    {
        options.UsePartitionRouter(partitionRouter);
        return this;
    }

    public BulkInsertEntityBuilder<T> UseDbContextFactory<TDbContext>()
        where TDbContext : DbContext
    {
        options.UseDbContextFactory<TDbContext>();
        return this;
    }

    public BulkInsertEntityBuilder<T> UseDbContextFactory(
        Func<IServiceProvider, DbContext> factory
    )
    {
        options.UseDbContextFactory(factory);
        return this;
    }

    public BulkInsertEntityBuilder<T> DisableReflectionFallback()
    {
        options.AllowReflectionFallback = false;
        return this;
    }

    public BulkInsertEntityBuilder<T> MapColumn<TProperty>(
        Expression<Func<T, TProperty>> selector,
        string columnName,
        NpgsqlDbType dbType,
        bool? isNullable = null
    )
    {
        options.MapColumn(selector, columnName, dbType, isNullable);
        return this;
    }
}
