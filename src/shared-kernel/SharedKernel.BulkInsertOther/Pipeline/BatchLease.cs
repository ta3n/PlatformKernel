using System.Buffers;
using System.Runtime.CompilerServices;

namespace SharedKernel.BulkInsertOther.Pipeline;

internal sealed class BatchLease<T>(
    int size,
    ArrayPool<T> pool
) : IDisposable
{
    public T[] Buffer { get; } = pool.Rent(size);

    public int Count { get; set; }

    public ReadOnlyMemory<T> Memory => Buffer.AsMemory(0, Count);

    public void Dispose()
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            Array.Clear(Buffer, 0, Count);
        }

        pool.Return(Buffer);
    }
}
