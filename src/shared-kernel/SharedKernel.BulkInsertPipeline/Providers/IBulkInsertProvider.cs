namespace SharedKernel.BulkInsertPipeline.Providers;

internal interface IBulkInsertProvider<T> : IBulkInsertService<T>
{
    BulkInsertProviderType ProviderType { get; }

    bool IsFallbackOnly { get; }

    Task InsertAsync(
        ReadOnlyMemory<T> batch,
        BulkInsertExecutionContext<T> context,
        CancellationToken ct = default
    );
}
