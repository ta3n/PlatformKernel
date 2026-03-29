namespace SharedKernel.BulkInsertPipeline.Abstractions;

public enum BulkInsertRejectionReason
{
    ChannelClosed = 0,
    CapacityExceeded = 1,
    CircuitOpen = 2,
    PermanentFailure = 3
}
