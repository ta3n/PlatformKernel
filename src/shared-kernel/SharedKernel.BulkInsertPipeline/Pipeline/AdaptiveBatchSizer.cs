using SharedKernel.BulkInsertPipeline.Options;

namespace SharedKernel.BulkInsertPipeline.Pipeline;

internal sealed class AdaptiveBatchSizer(
    AdaptiveBatchOptions options,
    int channelCapacity
)
{
    private readonly int _channelCapacity = Math.Max(1, channelCapacity);
    private int _currentBatchSize = options.MinBatchSize;

    public int GetNextSize(
        int queueDepth
    )
    {
        if (!options.Enabled)
        {
            return options.MaxBatchSize;
        }

        var occupancy = Math.Clamp((double)queueDepth / _channelCapacity, 0d, 1d);
        if (occupancy >= 0.75d)
        {
            _currentBatchSize = Math.Min(
                options.MaxBatchSize,
                (int)Math.Ceiling(_currentBatchSize * options.IncreaseMultiplier)
            );
        }
        else if (occupancy <= 0.20d)
        {
            _currentBatchSize = Math.Max(
                options.MinBatchSize,
                (int)Math.Floor(_currentBatchSize * options.DecreaseMultiplier)
            );
        }

        return _currentBatchSize;
    }

    public void Observe(
        int actualBatchSize,
        TimeSpan waitDuration
    )
    {
        if (!options.Enabled || _currentBatchSize <= options.MinBatchSize)
        {
            return;
        }

        var fillRatio = (double)actualBatchSize / _currentBatchSize;
        if (fillRatio < 0.35d && waitDuration >= options.MaxDelay)
        {
            _currentBatchSize = Math.Max(
                options.MinBatchSize,
                (int)Math.Floor(_currentBatchSize * options.DecreaseMultiplier)
            );
        }
    }
}
