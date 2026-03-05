using System.Collections.Concurrent;
using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Contexts.Interceptors;
using Liberty.Reservation.User.File.WebAPI.Application.ExternalServices.Membership.Facility;
using Liberty.Reservation.User.File.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.Reservation.User.File.WebAPI.Application.ExternalServices.Membership.Facility.Repositories.Implements;
using Liberty.Reservation.User.File.WebAPI.Application.Settings;
using Liberty.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.User.File.WebAPI.Configurations;

public static class DbContextExternalStartUp
{
    public static IServiceCollection AddExternalDbContexts(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.AddMembershipFacilityDbContext(config);

        return services;
    }

    private static void AddMembershipFacilityDbContext(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        var connectionPoolOptions = config.GetOptionsExt<ConnectionPoolOptions>("ConnectionPool");
        // var dbConnectionManager = new DbConnectionManager(connectionPoolOptions);
        // services.AddSingleton<IDbConnectionManager>(_ => dbConnectionManager);

        services.Configure<ConnectionPoolOptions>(config.GetSection("ConnectionPool"));
        services.Configure<ServiceSetting>(config.GetSection("Services"));
        var connectionExternal = config.GetOptionsExt<ServiceSetting>("Services");

        services.AddDbContextPool<MembershipFacilityExternalDbContext>(
            (
                serviceProvider,
                options
            ) =>
            {
                var concurrentQueue = serviceProvider.GetRequiredService<ConcurrentQueue<string>>();

                options
                    .UseNpgsql(
                        connectionExternal.MembershipFacilityService?.DataContextConnection,
                        sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                        }
                    )
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .LogTo(message => concurrentQueue.Enqueue(message), LogLevel.Information)
                    .EnableDetailedErrors()
                    .AddInterceptors(
                        // new CustomDbConnectionInterceptor(dbConnectionManager),
                        new SemaphoreDbConnectionInterceptor(
                            new SemaphoreSlim(
                                connectionPoolOptions.MaximumNumberOfConcurrentEntries
                            )
                        ),
                        new PerformanceInterceptor(
                            serviceProvider.GetRequiredService<ILogger<PerformanceInterceptor>>()
                        )
                    );

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging();
                }
            }
        );

        services.AddScoped<DbContext>(
            provider => provider.GetRequiredService<MembershipFacilityExternalDbContext>()
        );
    }

    public static IServiceCollection AddExternalRepositories(
        this IServiceCollection services
    )
    {
        services.AddMembershipFacilityRepositories();

        return services;
    }

    private static void AddMembershipFacilityRepositories(
        this IServiceCollection services
    )
    {
        services.AddScoped<IFacilityExternalRepository, FacilityExternalRepository>();
    }
}
