namespace SharedKernel.AuditLogging.Distributed.Models;

public sealed class AuditProcessingResult
{
    public AuditProcessingStatus Status { get; init; }

    public int AppendedLogs { get; init; }

    public static AuditProcessingResult Processed(
        int appendedLogs
    )
    {
        return new AuditProcessingResult
        {
            Status = AuditProcessingStatus.Processed,
            AppendedLogs = appendedLogs
        };
    }

    public static AuditProcessingResult Duplicate()
    {
        return new AuditProcessingResult
        {
            Status = AuditProcessingStatus.Duplicate
        };
    }

    public static AuditProcessingResult Pending()
    {
        return new AuditProcessingResult
        {
            Status = AuditProcessingStatus.Pending
        };
    }

    public static AuditProcessingResult Skipped()
    {
        return new AuditProcessingResult
        {
            Status = AuditProcessingStatus.Skipped
        };
    }
}
