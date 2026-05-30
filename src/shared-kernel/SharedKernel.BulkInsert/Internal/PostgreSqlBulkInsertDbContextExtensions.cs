using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace SharedKernel.BulkInsert.Internal;

internal static class PostgreSqlBulkInsertDbContextExtensions
{
    public static async Task<(NpgsqlConnection Connection, NpgsqlTransaction? Transaction, bool OpenedByScope)>
        GetOpenConnectionAsync(
            this DbContext dbContext,
            CancellationToken cancellationToken
        )
    {
        var connection = dbContext.Database.GetDbConnection() as NpgsqlConnection
            ?? throw new NotSupportedException("SharedKernel.BulkInsert only supports PostgreSQL via Npgsql.");

        var openedByScope = false;
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await dbContext.Database.OpenConnectionAsync(cancellationToken);
            openedByScope = true;
        }

        var transaction = dbContext.Database.CurrentTransaction?.GetDbTransaction() as NpgsqlTransaction;

        return (connection, transaction, openedByScope);
    }

    public static Task CloseConnectionIfNeededAsync(
        this DbContext dbContext,
        bool openedByScope
    )
    {
        return openedByScope
            ? dbContext.Database.CloseConnectionAsync()
            : Task.CompletedTask;
    }
}
