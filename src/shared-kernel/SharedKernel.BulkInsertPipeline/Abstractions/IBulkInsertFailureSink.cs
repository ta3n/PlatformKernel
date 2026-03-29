namespace SharedKernel.BulkInsertPipeline.Abstractions;

public interface IBulkInsertFailureSink<T>
{
    ValueTask OnRejectedAsync(
        T item,
        BulkInsertRejectionReason reason,
        CancellationToken cancellationToken = default
    );

    ValueTask OnBatchFailedAsync(
        ReadOnlyMemory<T> batch,
        Exception exception,
        CancellationToken cancellationToken = default
    );
}
