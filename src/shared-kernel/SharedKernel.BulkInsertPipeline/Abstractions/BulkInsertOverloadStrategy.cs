namespace SharedKernel.BulkInsertPipeline.Abstractions;

public enum BulkInsertOverloadStrategy
{
    Wait = 0,
    Drop = 1,
    Park = 2
}
