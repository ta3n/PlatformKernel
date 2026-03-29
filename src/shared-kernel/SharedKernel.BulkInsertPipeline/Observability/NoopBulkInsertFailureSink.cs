namespace SharedKernel.BulkInsertPipeline.Observability;

internal sealed class NoopBulkInsertFailureSink<T> : IBulkInsertFailureSink<T>
{
    public ValueTask OnRejectedAsync(
        T item,
        BulkInsertRejectionReason reason,
        CancellationToken cancellationToken = default
    )
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask OnBatchFailedAsync(
        ReadOnlyMemory<T> batch,
        Exception exception,
        CancellationToken cancellationToken = default
    )
    {
        return ValueTask.CompletedTask;
    }
}
