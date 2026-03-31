using Microsoft.EntityFrameworkCore;
using SharedKernel.BulkInsertPipeline.Options;

namespace SharedKernel.BulkInsertPipeline.Mapping;

public sealed class BulkInsertEntityDescriptor<T>
{
    public BulkInsertEntityDescriptor(
        BulkInsertEntityOptions<T> options
    )
    {
        Table = options.Table;
        Connection = options.Connection;
        Mapper = EntityColumnMapperFactory.Create(options);
        TimescaleTimeColumn = options.TimescaleTimeColumn;
        PartitionRouter = options.PartitionRouter;
        DbContextFactory = options.DbContextFactory;
        EntityName = typeof(T).Name;
        CopyCommand = BuildCopyCommand(Table);
    }

    public string EntityName { get; }

    public BulkInsertTableIdentifier Table { get; }

    public BulkInsertConnectionOptions Connection { get; }

    public IEntityColumnMapper<T> Mapper { get; }

    public string? TimescaleTimeColumn { get; }

    public IBulkInsertPartitionRouter<T>? PartitionRouter { get; }

    public Func<IServiceProvider, DbContext>? DbContextFactory { get; }

    public string CopyCommand { get; }

    public bool IsTimescaleHypertable => !string.IsNullOrWhiteSpace(TimescaleTimeColumn);

    public string BuildCopyCommand(
        BulkInsertTableIdentifier target
    )
    {
        var columns = string.Join(
            ", ",
            Mapper.Columns.Select(static column => BulkInsertTableIdentifier.QuoteIdentifier(column.ColumnName))
        );

        return $"COPY {target.QualifiedName} ({columns}) FROM STDIN (FORMAT BINARY)";
    }

    public string BuildInsertSql(
        int rowCount,
        BulkInsertTableIdentifier? target = null
    )
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rowCount);

        var effectiveTarget = target ?? Table;
        var quotedColumns = string.Join(
            ", ",
            Mapper.Columns.Select(static column => BulkInsertTableIdentifier.QuoteIdentifier(column.ColumnName))
        );

        var values = string.Join(
            ", ",
            Enumerable.Range(0, rowCount)
                .Select(
                    row =>
                        $"({string.Join(", ", Enumerable.Range(0, Mapper.Columns.Count).Select(column => $"@p{row}_{column}"))})"
                )
        );

        return $"INSERT INTO {effectiveTarget.QualifiedName} ({quotedColumns}) VALUES {values};";
    }

    public string BuildRawInsertSql(
        int rowCount,
        BulkInsertTableIdentifier? target = null
    )
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rowCount);

        var effectiveTarget = target ?? Table;
        var quotedColumns = string.Join(
            ", ",
            Mapper.Columns.Select(static column => BulkInsertTableIdentifier.QuoteIdentifier(column.ColumnName))
        );

        var values = string.Join(
            ", ",
            Enumerable.Range(0, rowCount)
                .Select(
                    row =>
                        $"({string.Join(", ", Enumerable.Range(0, Mapper.Columns.Count).Select(column => $"{{{(row * Mapper.Columns.Count) + column}}}"))})"
                )
        );

        return $"INSERT INTO {effectiveTarget.QualifiedName} ({quotedColumns}) VALUES {values};";
    }

    public int GetMaxRowsForParameterLimit(
        int maxParameters
    )
    {
        return Math.Max(1, maxParameters / Math.Max(1, Mapper.Columns.Count));
    }
}
