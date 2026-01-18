using System.Collections.Concurrent;
using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Contexts.Interceptors;
using Liberty.Reservation.Manager.Application;
using Liberty.Reservation.Manager.Application.Contexts;
using Liberty.Reservation.Manager.Application.Contexts.Interceptors;
using Liberty.Reservation.Manager.Application.Domains.Repositories;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork;
using Liberty.UnitOfWork.Abstractions;
using Liberty.UnitOfWork.Implementations;
using Liberty.UnitOfWork.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Manager.File.WebAPI.Configurations;

public static class DbContextStartup
{
    public static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.Configure<ConnectionPoolOptions>(config.GetSection("ConnectionPool"));
        var connectionPoolOptions = config.GetOptionsExt<ConnectionPoolOptions>("ConnectionPool");
        var connectionString = config.GetConnectionString("DataContextConnection");

        services.AddDbContextPool<ManagerDataContext>(
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
                            sqlOptions.EnableRetryOnFailure(
                                5,
                                TimeSpan.FromSeconds(30),
                                null
                            );
                        }
                    )
                    .UseSnakeCaseNamingConvention()
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .LogTo(message => concurrentQueue.Enqueue(message), LogLevel.Information)
                    .EnableDetailedErrors(false)
                    .EnableSensitiveDataLogging(false)
                    .AddInterceptors(
                        new AuditSaveChangesInterceptor(
                            serviceProvider.GetRequiredService<IHttpContextAccessor>()
                        ),
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
            provider => provider.GetRequiredService<ManagerDataContext>()
        );
        services.AddDbContextFactory<ManagerDataContext>();
        services.AddForwardingDbContextFactory<DbContext, ManagerDataContext>();

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
        var context = scope.ServiceProvider.GetRequiredService<ManagerDataContext>();

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

        services.AddScoped<IUnitOfWork, ManagerUnitOfWork>();

        services.AddScoped<IFacilityRepository, FacilityRepository>();
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IFacilityFileRepository, FacilityFileRepository>();
        services.AddScoped<IFileCategoryRepository, FileCategoryRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IFacilityCategoryRepository, FacilityCategoryRepository>();
        services.AddScoped<IFilePlanRepository, FilePlanRepository>();
        services.AddScoped<IFileRoomGroupRepository, FileRoomGroupRepository>();
        services.AddScoped<IFileOptionItemRepository, FileOptionItemRepository>();

        return services;
    }
}
