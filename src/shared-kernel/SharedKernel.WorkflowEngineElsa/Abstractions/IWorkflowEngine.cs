using SharedKernel.WorkflowEngineElsa.Models;

namespace SharedKernel.WorkflowEngineElsa.Abstractions;

/// <summary>
/// Starts workflow definitions without leaking Elsa runtime types into application services.
/// </summary>
public interface IWorkflowEngine
{
    Task<WorkflowStartResult> StartAsync(
        WorkflowStartRequest request,
        CancellationToken cancellationToken = default
    );
}
