using System.Collections.Concurrent;
using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Contexts.Interceptors;
using Liberty.Reservation.User.Application;
using Liberty.Reservation.User.Application.Contexts;
using Liberty.Reservation.User.Application.Contexts.Interceptors;
using Liberty.Reservation.User.Application.Domains.Repositories;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork;
using Liberty.UnitOfWork.Abstractions;
using Liberty.UnitOfWork.Implementations;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.User.File.WebAPI.Configurations;

public static class DbContextStartup
{
    public static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.Configure<ConnectionPoolOptions>(config.GetSection("ConnectionPool"));
        var connectionPoolOptions = config.GetOptionsExt<ConnectionPoolOptions>("ConnectionPool");
        // var dbConnectionManager = new DbConnectionManager(connectionPoolOptions);
        // services.AddSingleton<IDbConnectionManager>(_ => dbConnectionManager);

        var connectionString = config.GetConnectionString("DataContextConnection");

        services.AddDbContextPool<UserDataContext>(
            (
                serviceProvider,
                options
            ) =>
            {
                var concurrentQueue = serviceProvider.GetRequiredService<ConcurrentQueue<string>>();

                options.UseNpgsql(
                        connectionString,
                        sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                        }
                    )
                    .UseSnakeCaseNamingConvention()
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .LogTo(message => concurrentQueue.Enqueue(message), LogLevel.Information)
                    .EnableDetailedErrors(false)
                    .EnableSensitiveDataLogging(false)
                    .AddInterceptors(
                        // new CustomDbConnectionInterceptor(dbConnectionManager),
                        new SemaphoreDbConnectionInterceptor(
                            new SemaphoreSlim(
                                connectionPoolOptions.MaximumNumberOfConcurrentEntries
                            )
                        ),
                        new CacheSaveChangesInterceptor(serviceProvider),
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
            provider => provider.GetRequiredService<UserDataContext>()
        );
        services.AddDbContextFactory<UserDataContext>();
        services.AddForwardingDbContextFactory<DbContext, UserDataContext>();

        return services;
    }

    public static IApplicationBuilder UseApplicationDatabase(
        this IApplicationBuilder app,
        IHostEnvironment environment,
        bool isDbMigrationEnabled = false
    )
    {
        if (!isDbMigrationEnabled)
        {
            return app;
        }

        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UserDataContext>();

        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        return app;
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));

        services.AddScoped<IUnitOfWork, UserUnitOfWork>();

        services.AddScoped<IMediaRepository, MediaRepository>();
        services.AddScoped<IFacilityMediaRepository, FacilityMediaRepository>();

        return services;
    }
}
