using SharedKernel.Hangfire;

namespace SharedKernel.Hangfire.Models;

/// <summary>
/// Represents a target gRPC invocation requested by the scheduler executor.
/// </summary>
public sealed record SchedulerGrpcDispatchRequest
{
    /// <summary>
    /// Gets the logical target service name.
    /// </summary>
    public required string TargetService { get; init; }

    /// <summary>
    /// Gets the target gRPC method name.
    /// </summary>
    public required string GrpcMethod { get; init; }

    /// <summary>
    /// Gets the JSON payload forwarded to the target service.
    /// </summary>
    public required string PayloadJson { get; init; }

    /// <summary>
    /// Gets the scheduler execution metadata.
    /// </summary>
    public required SchedulerExecutionContext Execution { get; init; }

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
    /// Creates a dispatch request from a persisted job definition and execution context.
    /// </summary>
    /// <param name="definition">The persisted job definition.</param>
    /// <param name="execution">The execution context.</param>
    /// <returns>The dispatch request.</returns>
    public static SchedulerGrpcDispatchRequest Create(
        SchedulerJobDefinition definition,
        SchedulerExecutionContext execution
    )
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(execution);

        return new SchedulerGrpcDispatchRequest
        {
            TargetService = definition.TargetService,
            GrpcMethod = definition.GrpcMethod,
            PayloadJson = definition.PayloadJson,
            Execution = execution,
            SubSystemId = definition.SubSystemId,
            CompanyId = definition.CompanyId,
            ProjectId = definition.ProjectId,
            RequestedBy = definition.RequestedBy
        };
    }

    /// <summary>
    /// Builds outgoing gRPC metadata values that should be sent with the dispatch call.
    /// </summary>
    /// <returns>The metadata values keyed by lower-case gRPC metadata names.</returns>
    public IReadOnlyDictionary<string, string> ToGrpcMetadata()
    {
        var metadata = new Dictionary<string, string>
        {
            [SchedulerGrpcMetadataNames.SubSystemId] = SubSystemId,
            [SchedulerGrpcMetadataNames.CompanyId] = CompanyId,
            [SchedulerGrpcMetadataNames.ProjectId] = ProjectId,
            [SchedulerGrpcMetadataNames.IdempotencyKey] = Execution.IdempotencyKey,
            [SchedulerGrpcMetadataNames.ExecutionId] = Execution.ExecutionId,
            [SchedulerGrpcMetadataNames.ScheduledAt] = Execution.ScheduledAt.ToUniversalTime().ToString("O"),
            [SchedulerGrpcMetadataNames.RequestedBy] = RequestedBy
        };

        if (!string.IsNullOrWhiteSpace(Execution.CorrelationId))
        {
            metadata[SchedulerGrpcMetadataNames.CorrelationId] = Execution.CorrelationId;
        }

        return metadata;
    }
}
