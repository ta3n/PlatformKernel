namespace SharedKernel.BulkInsert.Internal;

internal sealed record PostgreSqlBulkInsertColumn<TEntity>(
    string PropertyName,
    string ColumnName,
    string QuotedColumnName,
    Type ProviderClrType,
    string? StoreType,
    Func<TEntity, object?> GetValue
)
    where TEntity : class;
