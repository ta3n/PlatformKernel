using Elsa.Extensions;
using Elsa.Features.Abstractions;
using Elsa.Features.Services;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.WorkflowEngineElsa.Abstractions;
using SharedKernel.WorkflowEngineElsa.Services;

namespace SharedKernel.WorkflowEngineElsa.Features;

/// <summary>
/// Registers shared-kernel Elsa activities and orchestration services as a reusable Elsa feature.
/// </summary>
public sealed class WorkflowEngineElsaFeature(
    IModule module
) : FeatureBase(module)
{
    public override void Configure()
    {
        Module.AddActivitiesFrom<WorkflowEngineElsaFeature>();
        Services.AddScoped<IWorkflowEngine, ElsaWorkflowEngine>();
        Services.AddScoped<IWorkflowSignalDispatcher, ElsaWorkflowSignalDispatcher>();
    }
}
