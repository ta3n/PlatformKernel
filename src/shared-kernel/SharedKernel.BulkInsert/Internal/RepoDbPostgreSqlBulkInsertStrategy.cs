using System.Data;
using Microsoft.EntityFrameworkCore;
using RepoDb;
using RepoDb.Enumerations.PostgreSql;
using SharedKernel.BulkInsert.Models;

namespace SharedKernel.BulkInsert.Internal;

internal sealed class RepoDbPostgreSqlBulkInsertStrategy : IPostgreSqlBulkInsertStrategy
{
    static RepoDbPostgreSqlBulkInsertStrategy()
    {
        GlobalConfiguration.Setup().UsePostgreSql();
    }

    public PostgreSqlBulkInsertProvider Provider => PostgreSqlBulkInsertProvider.RepoDb;

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

        var (connection, transaction, openedByScope) = await dbContext.GetOpenConnectionAsync(cancellationToken);

        try
        {
            var dataTable = CreateDataTable(entities, metadata);

            return await connection.BinaryBulkInsertAsync(
                metadata.QualifiedTableName,
                dataTable,
                null,
                null,
                options.TimeoutSeconds,
                options.BatchSize,
                BulkImportIdentityBehavior.Unspecified,
                BulkImportPseudoTableType.Temporary,
                transaction,
                cancellationToken
            );
        }
        finally
        {
            await dbContext.CloseConnectionIfNeededAsync(openedByScope);
        }
    }

    private static DataTable CreateDataTable<TEntity>(
        IReadOnlyCollection<TEntity> entities,
        PostgreSqlBulkInsertMetadata<TEntity> metadata
    )
        where TEntity : class
    {
        var dataTable = new DataTable(metadata.TableName);

        foreach (var column in metadata.Columns)
        {
            var dataColumn = new DataColumn(
                column.ColumnName,
                ResolveDataColumnType(column)
            )
            {
                AllowDBNull = true
            };
            dataTable.Columns.Add(dataColumn);
        }

        foreach (var entity in entities)
        {
            var row = dataTable.NewRow();

            foreach (var column in metadata.Columns)
            {
                row[column.ColumnName] = NormalizeValue(column, column.GetValue(entity)) ?? DBNull.Value;
            }

            dataTable.Rows.Add(row);
        }

        return dataTable;
    }

    private static Type ResolveDataColumnType<TEntity>(
        PostgreSqlBulkInsertColumn<TEntity> column
    )
        where TEntity : class
    {
        if (IsTimestampWithTimeZone(column))
        {
            return typeof(DateTimeOffset);
        }

        return Nullable.GetUnderlyingType(column.ProviderClrType) ?? column.ProviderClrType;
    }

    private static object? NormalizeValue<TEntity>(
        PostgreSqlBulkInsertColumn<TEntity> column,
        object? value
    )
        where TEntity : class
    {
        if (value is not DateTime dateTime)
        {
            return value;
        }

        if (IsTimestampWithTimeZone(column))
        {
            var utcDateTime = dateTime.Kind switch
            {
                DateTimeKind.Utc => dateTime,
                DateTimeKind.Local => dateTime.ToUniversalTime(),
                _ => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
            };

            return new DateTimeOffset(utcDateTime, TimeSpan.Zero);
        }

        if (IsTimestampWithoutTimeZone(column))
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
        }

        return value;
    }

    private static bool IsTimestampWithTimeZone<TEntity>(
        PostgreSqlBulkInsertColumn<TEntity> column
    )
        where TEntity : class
    {
        return column.ProviderClrType == typeof(DateTime)
            && column.StoreType is not null
            && (
                column.StoreType.Contains("timestamp with time zone", StringComparison.OrdinalIgnoreCase)
                || column.StoreType.Contains("timestamptz", StringComparison.OrdinalIgnoreCase)
            );
    }

    private static bool IsTimestampWithoutTimeZone<TEntity>(
        PostgreSqlBulkInsertColumn<TEntity> column
    )
        where TEntity : class
    {
        return column.ProviderClrType == typeof(DateTime)
            && column.StoreType is not null
            && (
                column.StoreType.Contains(
                    "timestamp without time zone",
                    StringComparison.OrdinalIgnoreCase
                )
                || string.Equals(column.StoreType, "timestamp", StringComparison.OrdinalIgnoreCase)
            );
    }
}
