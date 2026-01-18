namespace Liberty.ApplicationShared.Extensions;

public static class ListExtension
{
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
