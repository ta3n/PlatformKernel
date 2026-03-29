using NpgsqlTypes;

namespace SharedKernel.BulkInsertPipeline.Mapping;

[AttributeUsage(AttributeTargets.Property)]
public sealed class BulkInsertColumnAttribute(
    string columnName,
    NpgsqlDbType dbType
) : Attribute
{
    public string ColumnName { get; } = columnName;

    public NpgsqlDbType DbType { get; } = dbType;

    public bool IsNullable { get; init; }
}
