using NpgsqlTypes;

namespace SharedKernel.BulkInsertPipeline.Abstractions;

public readonly record struct BulkInsertColumn(
    string ColumnName,
    NpgsqlDbType DbType,
    bool IsNullable = false
);
