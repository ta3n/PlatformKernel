using Elsa.Workflows.Runtime;
using SharedKernel.WorkflowEngineElsa.Abstractions;
using SharedKernel.WorkflowEngineElsa.Models;

namespace SharedKernel.WorkflowEngineElsa.Services;

internal sealed class ElsaWorkflowSignalDispatcher(
    IStimulusSender stimulusSender
) : IWorkflowSignalDispatcher
{
    public Task DispatchAsync(
        WorkflowSignalRequest request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ActivityTypeName);

        var metadata = new StimulusMetadata { Input = request.Input };

        return request.Stimulus is null
            ? stimulusSender.SendAsync(request.ActivityTypeName, metadata, cancellationToken)
            : stimulusSender.SendAsync(request.ActivityTypeName, request.Stimulus, metadata, cancellationToken);
    }
}
