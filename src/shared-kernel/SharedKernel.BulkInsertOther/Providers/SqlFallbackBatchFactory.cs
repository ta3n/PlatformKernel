using Dapper;
using SharedKernel.BulkInsertOther.Mapping;

namespace SharedKernel.BulkInsertOther.Providers;

internal static class SqlFallbackBatchFactory
{
    public static IEnumerable<ReadOnlyMemory<T>> Chunk<T>(
        ReadOnlyMemory<T> batch,
        int chunkSize
    )
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(chunkSize);

        for (var offset = 0; offset < batch.Length; offset += chunkSize)
        {
            yield return batch.Slice(offset, Math.Min(chunkSize, batch.Length - offset));
        }
    }

    public static DynamicParameters CreateParameters<T>(
        BulkInsertEntityDescriptor<T> descriptor,
        ReadOnlyMemory<T> batch
    )
    {
        var values = ExtractValues(descriptor, batch);
        var parameters = new DynamicParameters();

        var valueIndex = 0;
        for (var row = 0; row < batch.Length; row++)
        {
            for (var column = 0; column < descriptor.Mapper.Columns.Count; column++)
            {
                parameters.Add($"p{row}_{column}", values[valueIndex++]);
            }
        }

        return parameters;
    }

    public static object?[] ExtractValues<T>(
        BulkInsertEntityDescriptor<T> descriptor,
        ReadOnlyMemory<T> batch
    )
    {
        var columnCount = descriptor.Mapper.Columns.Count;
        var values = new object?[batch.Length * columnCount];
        var span = batch.Span;

        for (var row = 0; row < span.Length; row++)
        {
            descriptor.Mapper.Extract(in span[row], values.AsSpan(row * columnCount, columnCount));
        }

        return values;
    }
}
