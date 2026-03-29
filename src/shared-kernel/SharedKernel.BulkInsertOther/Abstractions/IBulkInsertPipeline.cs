namespace SharedKernel.BulkInsertOther.Abstractions;

public interface IBulkInsertPipeline<in T>
{
    ValueTask EnqueueAsync(
        T item,
        CancellationToken cancellationToken = default
    );
}
