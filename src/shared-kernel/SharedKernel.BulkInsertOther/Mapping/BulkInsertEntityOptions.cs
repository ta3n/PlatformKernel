using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NpgsqlTypes;
using SharedKernel.BulkInsertOther.Options;
using SharedKernel.BulkInsertOther.Utilities;

namespace SharedKernel.BulkInsertOther.Mapping;

public sealed class BulkInsertEntityOptions<T>
{
    private readonly List<ColumnRegistration> _columns = [];

    public BulkInsertTableIdentifier Table { get; private set; } =
        new("public", BulkInsertNamingPolicy.ToSnakeCase(typeof(T).Name));

    public BulkInsertConnectionOptions Connection { get; } = new();

    public bool AllowReflectionFallback { get; set; } = true;

    public string? TimescaleTimeColumn { get; private set; }

    public IBulkInsertPartitionRouter<T>? PartitionRouter { get; private set; }

    internal IReadOnlyList<ColumnRegistration> Columns => _columns;

    internal Func<IServiceProvider, DbContext>? DbContextFactory { get; private set; }

    public void ToTable(
        string table,
        string schema = "public"
    )
    {
        Table = new BulkInsertTableIdentifier(schema, table);
    }

    public void ToHypertable(
        string table,
        string timeColumn,
        string schema = "public"
    )
    {
        Table = new BulkInsertTableIdentifier(schema, table);
        TimescaleTimeColumn = timeColumn;
    }

    public void UseDirectConnectionString(
        string connectionString
    )
    {
        Connection.DirectConnectionString = connectionString;
    }

    public void UseOltpConnectionString(
        string connectionString
    )
    {
        Connection.OltpConnectionString = connectionString;
    }

    public void UsePartitionRouter(
        IBulkInsertPartitionRouter<T> partitionRouter
    )
    {
        PartitionRouter = partitionRouter;
    }

    public void UseDbContextFactory<TDbContext>()
        where TDbContext : DbContext
    {
        DbContextFactory = sp =>
            sp.GetRequiredService<IDbContextFactory<TDbContext>>().CreateDbContext();
    }

    public void UseDbContextFactory(
        Func<IServiceProvider, DbContext> factory
    )
    {
        DbContextFactory = factory;
    }

    public void MapColumn<TProperty>(
        Expression<Func<T, TProperty>> selector,
        string columnName,
        NpgsqlDbType dbType,
        bool? isNullable = null
    )
    {
        _columns.Add(
            new ColumnRegistration(
                selector,
                typeof(TProperty),
                columnName,
                dbType,
                isNullable
            )
        );
    }

    internal sealed record ColumnRegistration(
        LambdaExpression Selector,
        Type PropertyType,
        string ColumnName,
        NpgsqlDbType DbType,
        bool? IsNullable
    );
}
