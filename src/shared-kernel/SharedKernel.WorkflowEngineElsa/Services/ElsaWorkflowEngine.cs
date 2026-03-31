using Elsa.Workflows.Models;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Messages;
using SharedKernel.WorkflowEngineElsa.Abstractions;
using SharedKernel.WorkflowEngineElsa.Models;

namespace SharedKernel.WorkflowEngineElsa.Services;

internal sealed class ElsaWorkflowEngine(
    IWorkflowRuntime workflowRuntime
) : IWorkflowEngine
{
    public async Task<WorkflowStartResult> StartAsync(
        WorkflowStartRequest request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkflowDefinitionId);

        var client = await workflowRuntime.CreateClientAsync(cancellationToken);
        var result = await client.CreateAndRunInstanceAsync(
            new CreateAndRunWorkflowInstanceRequest
            {
                WorkflowDefinitionHandle = WorkflowDefinitionHandle.ByDefinitionId(request.WorkflowDefinitionId),
                Input = request.Input,
                CorrelationId = request.CorrelationId
            },
            cancellationToken
        );
        var workflowState = request.IncludeWorkflowOutput
            ? await (await workflowRuntime.CreateClientAsync(result.WorkflowInstanceId, cancellationToken))
                .ExportStateAsync(cancellationToken)
            : null;

        return new WorkflowStartResult
        {
            WorkflowInstanceId = result.WorkflowInstanceId,
            WorkflowDefinitionId = workflowState?.DefinitionId ?? request.WorkflowDefinitionId,
            CorrelationId = workflowState?.CorrelationId ?? request.CorrelationId,
            Status = workflowState?.Status.ToString() ?? result.Status.ToString(),
            Output = workflowState?.Output
        };
    }
}
