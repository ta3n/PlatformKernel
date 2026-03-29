namespace SharedKernel.BulkInsert.Models;

public sealed class PostgreSqlBulkInsertOptions
{
    public int? BatchSize { get; init; }

    public int? TimeoutSeconds { get; init; }
}
