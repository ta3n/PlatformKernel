using NpgsqlTypes;

namespace SharedKernel.BulkInsertOther.Abstractions;

public readonly record struct BulkInsertColumn(
    string ColumnName,
    NpgsqlDbType DbType,
    bool IsNullable = false
);
