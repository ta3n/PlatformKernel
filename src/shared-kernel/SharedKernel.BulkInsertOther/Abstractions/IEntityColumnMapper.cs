using Npgsql;

namespace SharedKernel.BulkInsertOther.Abstractions;

public interface IEntityColumnMapper<T>
{
    IReadOnlyList<BulkInsertColumn> Columns { get; }

    void Write(
        in T entity,
        NpgsqlBinaryImporter writer
    );

    ValueTask WriteAsync(
        T entity,
        NpgsqlBinaryImporter writer,
        CancellationToken ct = default
    );

    void Extract(
        in T entity,
        Span<object?> destination
    );
}
