using SharedKernel.WorkflowEngineElsa.Models;

namespace SharedKernel.WorkflowEngineElsa.Abstractions;

/// <summary>
/// Resumes or triggers workflows waiting on Elsa bookmarks and triggers.
/// </summary>
public interface IWorkflowSignalDispatcher
{
    Task DispatchAsync(
        WorkflowSignalRequest request,
        CancellationToken cancellationToken = default
    );
}
