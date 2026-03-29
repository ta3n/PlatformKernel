namespace SharedKernel.BulkInsert.Internal;

internal static class PostgreSqlBulkInsertSqlHelper
{
    public const int PostgreSqlParameterLimit = 65535;

    public static string QuoteIdentifier(
        string identifier
    )
    {
        return $"\"{identifier.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
    }

    public static string GetQualifiedTableName(
        string? schema,
        string tableName
    )
    {
        return string.IsNullOrWhiteSpace(schema)
            ? tableName
            : $"{schema}.{tableName}";
    }

    public static string GetQuotedQualifiedTableName(
        string? schema,
        string tableName
    )
    {
        var quotedTableName = QuoteIdentifier(tableName);

        return string.IsNullOrWhiteSpace(schema)
            ? quotedTableName
            : $"{QuoteIdentifier(schema)}.{quotedTableName}";
    }

    public static int ResolveBatchSize(
        int? requestedBatchSize,
        int rowCount,
        int columnCount
    )
    {
        if (rowCount <= 0)
        {
            return 0;
        }

        if (columnCount <= 0)
        {
            throw new InvalidOperationException("Bulk insert requires at least one mapped column.");
        }

        var maxBatchSizeByParameterLimit = Math.Max(1, PostgreSqlParameterLimit / columnCount);
        var effectiveBatchSize = requestedBatchSize ?? rowCount;

        if (effectiveBatchSize <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(requestedBatchSize),
                "BatchSize must be greater than zero."
            );
        }

        return Math.Min(effectiveBatchSize, maxBatchSizeByParameterLimit);
    }
}
