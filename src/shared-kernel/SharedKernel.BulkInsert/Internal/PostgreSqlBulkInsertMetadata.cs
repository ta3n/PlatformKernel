namespace SharedKernel.BulkInsert.Internal;

internal sealed record PostgreSqlBulkInsertMetadata<TEntity>(
    string? Schema,
    string TableName,
    string QualifiedTableName,
    string QuotedQualifiedTableName,
    IReadOnlyList<PostgreSqlBulkInsertColumn<TEntity>> Columns
)
    where TEntity : class;
