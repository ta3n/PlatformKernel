namespace PlatformKernel.ApplicationShared.Extensions;

/// <summary>
/// Provides extension methods for <see cref="List{T}"/>.
/// </summary>
public static class ListExtension
{
    /// <summary>
    /// Splits a list into multiple smaller batches of a specified size.
    /// </summary>
    /// <typeparam name="T">The type of objects in the list. Must be a reference type.</typeparam>
    /// <param name="data">The source list to be split into batches.</param>
    /// <param name="batchSize">The maximum number of elements in each batch. Defaults to 1000.</param>
    /// <returns>An enumerable of enumerable batches, where each batch contains elements from the source list.</returns>
    public static IEnumerable<IEnumerable<T>> SplitBatch<T>(
        this List<T> data,
        int batchSize = 1000
    )
        where T : class
    {
        var result = new List<IEnumerable<T>>();
        var batchNumber = (int)Math.Ceiling(data.Count / (decimal)batchSize);

        for (var i = 0; i < batchNumber; i++)
        {
            var batch = data
                .Skip(i * batchSize)
                .Take(batchSize);

            result.Add(batch);
        }

        return result;
    }
}
