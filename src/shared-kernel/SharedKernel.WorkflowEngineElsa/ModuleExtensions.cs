using Elsa.Extensions;
using Elsa.Features.Services;
using SharedKernel.WorkflowEngineElsa.Features;

namespace SharedKernel.WorkflowEngineElsa;

/// <summary>
/// Adds the shared-kernel Elsa feature to an Elsa module.
/// </summary>
public static class ModuleExtensions
{
    public static IModule UseSharedKernelWorkflowEngineElsa(
        this IModule module,
        Action<WorkflowEngineElsaFeature>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(module);

        module.Use(configure);
        return module;
    }
}
