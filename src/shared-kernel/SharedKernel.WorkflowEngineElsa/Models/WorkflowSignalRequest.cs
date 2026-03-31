namespace SharedKernel.WorkflowEngineElsa.Models;

/// <summary>
/// Describes a stimulus sent to start or resume workflows.
/// </summary>
public sealed record WorkflowSignalRequest
{
    public required string ActivityTypeName { get; init; }

    public object? Stimulus { get; init; }

    public IDictionary<string, object>? Input { get; init; }
}
