namespace SharedKernel.WorkflowEngineElsa.Models;

/// <summary>
/// Describes a workflow instance started through the shared-kernel abstraction.
/// </summary>
public sealed record WorkflowStartResult
{
    public required string WorkflowInstanceId { get; init; }

    public required string WorkflowDefinitionId { get; init; }

    public string? CorrelationId { get; init; }

    public string? Status { get; init; }

    public IDictionary<string, object>? Output { get; init; }
}
