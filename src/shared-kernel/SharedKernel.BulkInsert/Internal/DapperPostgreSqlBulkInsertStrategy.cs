using System.Text;
using Dapper;
using Microsoft.EntityFrameworkCore;
using SharedKernel.BulkInsert.Models;

namespace SharedKernel.BulkInsert.Internal;

internal sealed class DapperPostgreSqlBulkInsertStrategy : IPostgreSqlBulkInsertStrategy
{
    public PostgreSqlBulkInsertProvider Provider => PostgreSqlBulkInsertProvider.Dapper;

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

        var batchSize = PostgreSqlBulkInsertSqlHelper.ResolveBatchSize(
            options.BatchSize,
            entities.Count,
            metadata.Columns.Count
        );
        var (connection, currentTransaction, openedByScope) = await dbContext.GetOpenConnectionAsync(cancellationToken);
        var ownsTransaction = currentTransaction is null;
        await using var transaction = ownsTransaction
            ? await connection.BeginTransactionAsync(cancellationToken)
            : null;
        var effectiveTransaction = transaction ?? currentTransaction;

        try
        {
            var totalAffectedRows = 0;

            foreach (var batch in entities.Chunk(batchSize))
            {
                var parameters = new DynamicParameters();
                var commandText = BuildInsertCommandText(batch, metadata, parameters);

                totalAffectedRows += await connection.ExecuteAsync(
                    new CommandDefinition(
                        commandText,
                        parameters,
                        effectiveTransaction,
                        options.TimeoutSeconds,
                        cancellationToken: cancellationToken
                    )
                );
            }

            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return totalAffectedRows;
        }
        catch
        {
            if (transaction is not null)
            {
                await transaction.RollbackAsync(cancellationToken);
            }

            throw;
        }
        finally
        {
            await dbContext.CloseConnectionIfNeededAsync(openedByScope);
        }
    }

    private static string BuildInsertCommandText<TEntity>(
        IReadOnlyCollection<TEntity> batch,
        PostgreSqlBulkInsertMetadata<TEntity> metadata,
        DynamicParameters parameters
    )
        where TEntity : class
    {
        var builder = new StringBuilder();
        builder.Append("INSERT INTO ");
        builder.Append(metadata.QuotedQualifiedTableName);
        builder.Append(" (");
        builder.Append(string.Join(", ", metadata.Columns.Select(column => column.QuotedColumnName)));
        builder.Append(") VALUES ");

        var rowIndex = 0;
        foreach (var entity in batch)
        {
            if (rowIndex > 0)
            {
                builder.Append(", ");
            }

            builder.Append('(');

            for (var columnIndex = 0; columnIndex < metadata.Columns.Count; columnIndex++)
            {
                if (columnIndex > 0)
                {
                    builder.Append(", ");
                }

                var column = metadata.Columns[columnIndex];
                var parameterName = $"p_{rowIndex}_{column.PropertyName}";
                builder.Append('@');
                builder.Append(parameterName);
                parameters.Add(parameterName, column.GetValue(entity));
            }

            builder.Append(')');
            rowIndex++;
        }

        builder.Append(';');

        return builder.ToString();
    }
}
