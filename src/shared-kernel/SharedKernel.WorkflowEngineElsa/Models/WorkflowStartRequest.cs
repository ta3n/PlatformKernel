namespace SharedKernel.WorkflowEngineElsa.Models;

/// <summary>
/// Represents the minimal data required to start an Elsa workflow definition.
/// </summary>
public sealed record WorkflowStartRequest
{
    public required string WorkflowDefinitionId { get; init; }

    public IDictionary<string, object> Input { get; init; } = new Dictionary<string, object>();

    public string? CorrelationId { get; init; }

    public bool IncludeWorkflowOutput { get; init; }
}
