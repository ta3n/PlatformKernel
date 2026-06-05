using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharedKernel.AuditLogging.Abstractions;
using SharedKernel.AuditLogging.Distributed.Abstractions;
using SharedKernel.AuditLogging.Distributed.Consumers;
using SharedKernel.AuditLogging.Distributed.Interceptors;
using SharedKernel.AuditLogging.Distributed.Options;
using SharedKernel.AuditLogging.Distributed.Services;
using SharedKernel.AuditLogging.Services;

namespace SharedKernel.AuditLogging.Distributed.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedAuditProducer(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = AuditProducerOptions.SectionName
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);

        services.AddDistributedAuditProducerCore();
        services.AddOptions<AuditProducerOptions>()
            .Bind(configuration.GetSection(sectionName))
            .Validate(
                static options => options.SchemaVersion > 0,
                "AuditProducer:SchemaVersion must be greater than zero."
            );

        return services;
    }

    public static IServiceCollection AddDistributedAuditProducer(
        this IServiceCollection services,
        Action<AuditProducerOptions>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddDistributedAuditProducerCore();
        if (configure is not null)
        {
            services.Configure(configure);
        }

        return services;
    }

    public static IServiceCollection AddDistributedAuditService<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = AuditServiceOptions.SectionName
    ) where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);

        services.AddDistributedAuditServiceCore<TDbContext>();
        services.AddOptions<AuditServiceOptions>()
            .Bind(configuration.GetSection(sectionName));

        return services;
    }

    public static IServiceCollection AddDistributedAuditService<TDbContext>(
        this IServiceCollection services,
        Action<AuditServiceOptions>? configure = null
    ) where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddDistributedAuditServiceCore<TDbContext>();
        if (configure is not null)
        {
            services.Configure(configure);
        }

        return services;
    }

    private static void AddDistributedAuditProducerCore(
        this IServiceCollection services
    )
    {
        services.AddHttpContextAccessor();
        services.TryAddSingleton<IAuditContextAccessor, DefaultAuditContextAccessor>();
        services.TryAddSingleton<IAuditEntityIdResolver, DefaultAuditEntityIdResolver>();
        services.TryAddSingleton<IAuditEntityVersionResolver, DefaultAuditEntityVersionResolver>();
        services.TryAddScoped<AuditEntityChangedInterceptor>();
    }

    private static void AddDistributedAuditServiceCore<TDbContext>(
        this IServiceCollection services
    ) where TDbContext : DbContext
    {
        services.TryAddScoped<AuditEntityChangedProcessor>();
        services.TryAddScoped<AuditEntityChangedConsumer<TDbContext>>();
    }
}
