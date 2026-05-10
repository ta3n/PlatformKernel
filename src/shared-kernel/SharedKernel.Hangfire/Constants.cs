namespace SharedKernel.Hangfire;

/// <summary>
/// Contains lower-case gRPC metadata names for scheduler dispatch.
/// </summary>
public static class SchedulerGrpcMetadataNames
{
    /// <summary>
    /// Tenant subsystem id metadata name.
    /// </summary>
    public const string SubSystemId = "x-subsystem-id";

    /// <summary>
    /// Tenant company id metadata name.
    /// </summary>
    public const string CompanyId = "x-company-id";

    /// <summary>
    /// Tenant project id metadata name.
    /// </summary>
    public const string ProjectId = "x-project-id";

    /// <summary>
    /// Scheduler idempotency key metadata name.
    /// </summary>
    public const string IdempotencyKey = "x-idempotency-key";

    /// <summary>
    /// Scheduler execution id metadata name.
    /// </summary>
    public const string ExecutionId = "x-execution-id";

    /// <summary>
    /// Scheduler due timestamp metadata name.
    /// </summary>
    public const string ScheduledAt = "x-scheduled-at";

    /// <summary>
    /// Correlation id or trace id metadata name.
    /// </summary>
    public const string CorrelationId = "x-correlation-id";

    /// <summary>
    /// Requesting user or service metadata name.
    /// </summary>
    public const string RequestedBy = "x-requested-by";
}
