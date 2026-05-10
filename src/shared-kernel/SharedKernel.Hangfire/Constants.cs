namespace SharedKernel.Hangfire;

/// <summary>
/// Contains constants for API endpoints used in job registration processes.
/// </summary>
public static class RegisterJobEndpoint
{
    /// <summary>
    /// Represents the endpoint URL for registering recurring jobs in the system.
    /// </summary>
    public const string RegisterRecurringJobEndpoint = "api/register/recurring-jobs";

    /// <summary>
    /// Represents the endpoint used to register a scheduled job to execute at a specific time.
    /// </summary>
    /// <remarks>
    /// This constant defines the endpoint used by the application to interact with the BatchSchedulerService
    /// for scheduling jobs that execute at a specific time. It is commonly utilized in methods responsible
    /// for calling the external API service to schedule tasks.
    /// </remarks>
    public const string RegisterScheduleJobAtEndpoint = "api/register/schedule-jobs/at";

    /// <summary>
    /// Represents the endpoint URL for scheduling delayed jobs in the batch job scheduler service.
    /// This constant is used for sending requests to schedule jobs with a delay through
    /// the external batch scheduler API.
    /// </summary>
    public const string RegisterScheduleJobDelayEndpoint = "api/register/schedule-jobs/delay";
}

/// <summary>
/// Contains scheduler HTTP header names used across service boundaries.
/// </summary>
public static class SchedulerHeaderNames
{
    /// <summary>
    /// Tenant subsystem id header.
    /// </summary>
    public const string SubSystemId = "X-SubSystem-Id";

    /// <summary>
    /// Tenant company id header.
    /// </summary>
    public const string CompanyId = "X-Company-Id";

    /// <summary>
    /// Tenant project id header.
    /// </summary>
    public const string ProjectId = "X-Project-Id";

    /// <summary>
    /// Scheduler idempotency key header.
    /// </summary>
    public const string IdempotencyKey = "X-Idempotency-Key";

    /// <summary>
    /// Scheduler execution id header.
    /// </summary>
    public const string ExecutionId = "X-Execution-Id";

    /// <summary>
    /// Scheduler due timestamp header.
    /// </summary>
    public const string ScheduledAt = "X-Scheduled-At";

    /// <summary>
    /// Correlation id or trace id header.
    /// </summary>
    public const string CorrelationId = "X-Correlation-Id";

    /// <summary>
    /// Requesting user or service header.
    /// </summary>
    public const string RequestedBy = "X-Requested-By";
}

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
