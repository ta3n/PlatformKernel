using Elsa.EntityFrameworkCore;
using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Elsa.Features.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.WorkflowEngineElsa.Options;
using ManagementMigrations = Elsa.EntityFrameworkCore.PostgreSql.Migrations.Management;
using RuntimeMigrations = Elsa.EntityFrameworkCore.PostgreSql.Migrations.Runtime;

namespace SharedKernel.WorkflowEngineElsa;

/// <summary>
/// Provides service collection and application builder extensions for Elsa workflow engine integration.
/// </summary>
public static class Extensions
{
    public static IServiceCollection AddWorkflowEngineElsa(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IModule>? configureModule = null
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(WorkflowEngineElsaOptions.SectionName);
        var options = section.Get<WorkflowEngineElsaOptions>() ?? new WorkflowEngineElsaOptions();
        var module = services.CreateModule();

        services.Configure<WorkflowEngineElsaOptions>(section);
        module.UseSharedKernelWorkflowEngineElsa();

        if (options.UseEntityFrameworkPersistence)
        {
            ConfigureManagementPersistence(module, configuration, options);
            ConfigureRuntimePersistence(module, configuration, options);
        }

        if (options.EnableScheduling)
        {
            module.UseScheduling();
        }

        if (options.EnableHttpActivities)
        {
            module.UseHttp();
        }

        if (options.ExposeWorkflowsApi)
        {
            module.UseWorkflowsApi();
        }

        configureModule?.Invoke(module);
        module.Apply();

        return services;
    }

    public static IApplicationBuilder UseWorkflowEngineElsa(
        this IApplicationBuilder app,
        IConfiguration configuration
    )
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(configuration);

        var options = configuration
                .GetSection(WorkflowEngineElsaOptions.SectionName)
                .Get<WorkflowEngineElsaOptions>()
            ?? new WorkflowEngineElsaOptions();

        if (options.ExposeWorkflowsApi || options.EnableHttpActivities)
        {
            app.UseWorkflows();
        }

        return app;
    }

    private static void ConfigureManagementPersistence(
        IModule elsa,
        IConfiguration configuration,
        WorkflowEngineElsaOptions options
    )
    {
        var connectionString = ResolveManagementConnectionString(configuration, options);

        elsa.UseWorkflowManagement(
            management =>
            {
                management.UseEntityFrameworkCore(
                    ef =>
                    {
                        ef.RunMigrations = options.RunMigrations;
                        ef.DbContextOptionsBuilder = (
                                _,
                                builder
                            ) =>
                            builder.UseElsaPostgreSql(
                                typeof(ManagementMigrations.Initial).Assembly,
                                connectionString,
                                new ElsaDbContextOptions(),
                                _ => { }
                            );
                    }
                );
            }
        );
    }

    private static void ConfigureRuntimePersistence(
        IModule elsa,
        IConfiguration configuration,
        WorkflowEngineElsaOptions options
    )
    {
        var connectionString = ResolveRuntimeConnectionString(configuration, options);

        elsa.UseWorkflowRuntime(
            runtime =>
            {
                runtime.UseEntityFrameworkCore(
                    ef =>
                    {
                        ef.RunMigrations = options.RunMigrations;
                        ef.DbContextOptionsBuilder = (
                                _,
                                builder
                            ) =>
                            builder.UseElsaPostgreSql(
                                typeof(RuntimeMigrations.Initial).Assembly,
                                connectionString,
                                new ElsaDbContextOptions(),
                                _ => { }
                            );
                    }
                );
            }
        );
    }

    private static string ResolveManagementConnectionString(
        IConfiguration configuration,
        WorkflowEngineElsaOptions options
    )
    {
        var connectionStringName = options.ManagementConnectionStringName ?? options.ConnectionStringName;
        return configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{connectionStringName}' was not found for Elsa workflow management persistence."
            );
    }

    private static string ResolveRuntimeConnectionString(
        IConfiguration configuration,
        WorkflowEngineElsaOptions options
    )
    {
        var connectionStringName = options.RuntimeConnectionStringName ?? options.ConnectionStringName;
        return configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{connectionStringName}' was not found for Elsa workflow runtime persistence."
            );
    }
}
