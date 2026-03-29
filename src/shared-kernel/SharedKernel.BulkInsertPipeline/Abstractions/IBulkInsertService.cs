namespace SharedKernel.BulkInsertPipeline.Abstractions;

public interface IBulkInsertService<T>
{
    Task InsertAsync(
        ReadOnlyMemory<T> batch,
        CancellationToken ct = default
    );
}
