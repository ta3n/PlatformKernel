namespace SharedKernel.WorkflowEngineElsa.Options;

/// <summary>
/// Configures the shared-kernel Elsa workflow engine integration.
/// </summary>
public sealed class WorkflowEngineElsaOptions
{
    public const string SectionName = "WorkflowEngineElsa";

    /// <summary>
    /// Enables Entity Framework Core persistence for workflow management and runtime stores.
    /// </summary>
    public bool UseEntityFrameworkPersistence { get; set; } = true;

    /// <summary>
    /// Applies Elsa EF Core migrations on startup. Keep disabled in production unless startup migrations are intentional.
    /// </summary>
    public bool RunMigrations { get; set; }

    /// <summary>
    /// Gets or sets the default connection string name used for both management and runtime stores.
    /// </summary>
    public string ConnectionStringName { get; set; } = "WorkflowEngineElsa";

    /// <summary>
    /// Gets or sets an optional dedicated management store connection string name.
    /// </summary>
    public string? ManagementConnectionStringName { get; set; }

    /// <summary>
    /// Gets or sets an optional dedicated runtime store connection string name.
    /// </summary>
    public string? RuntimeConnectionStringName { get; set; }

    /// <summary>
    /// Enables Elsa scheduling activities.
    /// </summary>
    public bool EnableScheduling { get; set; } = true;

    /// <summary>
    /// Enables Elsa HTTP activities.
    /// </summary>
    public bool EnableHttpActivities { get; set; }

    /// <summary>
    /// Exposes Elsa workflow API endpoints.
    /// </summary>
    public bool ExposeWorkflowsApi { get; set; }
}
