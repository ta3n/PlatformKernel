namespace SharedKernel.AuditLogging.Distributed.Models;

public enum AuditProcessingStatus
{
    Processed = 1,
    Duplicate = 2,
    Pending = 3,
    Skipped = 4
}
