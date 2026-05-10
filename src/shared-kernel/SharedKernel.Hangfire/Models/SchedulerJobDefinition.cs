namespace SharedKernel.Hangfire.Models;

/// <summary>
/// Represents scheduler metadata stored outside Hangfire and used by the generic executor.
/// </summary>
public sealed record SchedulerJobDefinition
{
    /// <summary>
    /// Gets the stable unique scheduler key.
    /// </summary>
    public required string JobKey { get; init; }

    /// <summary>
    /// Gets the logical target service name.
    /// </summary>
    public required string TargetService { get; init; }

    /// <summary>
    /// Gets the target gRPC method name.
    /// </summary>
    public required string GrpcMethod { get; init; }

    /// <summary>
    /// Gets the cron expression for recurring jobs.
    /// </summary>
    public string? CronExpression { get; init; }

    /// <summary>
    /// Gets the JSON payload forwarded to the target service.
    /// </summary>
    public required string PayloadJson { get; init; }

    /// <summary>
    /// Gets the tenant subsystem id.
    /// </summary>
    public required string SubSystemId { get; init; }

    /// <summary>
    /// Gets the tenant company id.
    /// </summary>
    public required string CompanyId { get; init; }

    /// <summary>
    /// Gets the tenant project id.
    /// </summary>
    public required string ProjectId { get; init; }

    /// <summary>
    /// Gets the caller that requested or owns the job.
    /// </summary>
    public required string RequestedBy { get; init; }

    /// <summary>
    /// Gets the recurring schedule time zone id.
    /// </summary>
    public string TimeZoneId { get; init; } = TimeZoneInfo.Utc.Id;

    /// <summary>
    /// Gets the scheduler status.
    /// </summary>
    public SchedulerJobStatus Status { get; init; } = SchedulerJobStatus.Active;
}
