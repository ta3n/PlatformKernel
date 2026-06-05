using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SharedKernel.AuditLogging.Abstractions;
using SharedKernel.AuditLogging.Interceptors;
using SharedKernel.AuditLogging.Models;
using SharedKernel.AuditLogging.Options;
using SharedKernel.AuditLogging.Services;

namespace SharedKernel.AuditLogging.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuditLogging(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = AuditLoggingOptions.SectionName
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);

        services.AddAuditLoggingCore();
        services.AddOptions<AuditLoggingOptions>()
            .Bind(configuration.GetSection(sectionName))
            .Validate(static options => options.OutboxBatchSize > 0, "AuditLogging:OutboxBatchSize must be greater than zero.")
            .Validate(
                static options => options.OutboxPollingInterval > TimeSpan.Zero,
                "AuditLogging:OutboxPollingInterval must be greater than zero."
            );

        return services;
    }

    public static IServiceCollection AddAuditLogging(
        this IServiceCollection services,
        Action<AuditLoggingOptions> configure
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services.AddAuditLoggingCore();
        services.Configure(configure);

        return services;
    }

    public static IServiceCollection AddAuditOutboxProcessor<TDbContext>(
        this IServiceCollection services
    ) where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHostedService<AuditOutboxBackgroundService<TDbContext>>();
        return services;
    }

    private static void AddAuditLoggingCore(
        this IServiceCollection services
    )
    {
        services.AddHttpContextAccessor();
        services.TryAddSingleton<IAuditContextAccessor, DefaultAuditContextAccessor>();
        services.TryAddSingleton<IAuditEntityIdResolver, DefaultAuditEntityIdResolver>();
        services.TryAddScoped<EfCoreAuditSink>();
        services.TryAddScoped<EfCoreAuditOutboxSink>();
        services.TryAddScoped<AuditOutboxProcessor>();
        services.TryAddScoped<AuditSaveChangesTrailInterceptor>();
        services.TryAddScoped<IAuditSink>(
            static serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<AuditLoggingOptions>>().Value;
                return options.Mode == AuditLoggingMode.Outbox
                    ? serviceProvider.GetRequiredService<EfCoreAuditOutboxSink>()
                    : serviceProvider.GetRequiredService<EfCoreAuditSink>();
            }
        );
    }
}
