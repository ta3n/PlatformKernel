using System.Collections.Concurrent;
using PlatformKernel.Service.Application;
using PlatformKernel.Service.Application.Contexts;
using PlatformKernel.Service.Application.Contexts.Interceptors;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PlatformKernel.ApplicationShared.Domains.Repositories;
using PlatformKernel.ApplicationShared.Extensions;
using PlatformKernel.Service.Base.Application.Behaviors;
using PlatformKernel.Service.Base.Application.Contexts.Interceptors;
using PlatformKernel.UnitOfWork;
using PlatformKernel.UnitOfWork.Abstractions;
using PlatformKernel.UnitOfWork.Implementations;
using PlatformKernel.UnitOfWork.Interceptors;
using Serilog;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace PlatformKernel.Service.WebApi.Configurations;

public static class DbContextStartup
{
    public static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        // Configure and retrieve options
        services.Configure<ConnectionPoolOptions>(config.GetSection("ConnectionPool"));
        var connectionPoolOptions = config.GetOptionsExt<ConnectionPoolOptions>("ConnectionPool");
        var connectionString = config.GetConnectionString("DataContextConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DataContextConnection' is not configured."
            );
        var connectionReadDataString = config.GetConnectionString("DataReadContextConnection") ?? string.Empty;

        // Register DbContexts with specific configurations
        ConfigureWriteDataContext(
            services,
            connectionString,
            connectionPoolOptions
        );
        ConfigureReadDataContext(
            services,
            connectionReadDataString,
            connectionPoolOptions
        );

        // Register related services
        RegisterDbContextServices(services);

        services.AddScoped(
            _ =>
            {
                var connection = new NpgsqlConnection(connectionString);
                var compiler = new PostgresCompiler();

                var queryFactory = new QueryFactory(connection, compiler)
                {
                    Logger = compiled =>
                    {
                        Log.Information(
                            "Executing SQL: {Sql} with bindings: {@Bindings}",
                            compiled.Sql,
                            compiled.Bindings
                        );
                    }
                };

                return queryFactory;
            }
        );

        return services;
    }

    private static void ConfigureWriteDataContext(
        IServiceCollection services,
        string connectionString,
        ConnectionPoolOptions connectionPoolOptions
    )
    {
        services.AddDbContextPool<AppWriteDataContext>(
            (
                serviceProvider,
                options
            ) =>
            {
                ConfigureCommonDbContextOptions(
                    options,
                    serviceProvider,
                    connectionString,
                    connectionPoolOptions
                );

                options.AddInterceptors(
                    new CacheSaveChangesInterceptor(serviceProvider)
                );
            }
        );
    }

    private static void ConfigureReadDataContext(
        IServiceCollection services,
        string connectionString,
        ConnectionPoolOptions connectionPoolOptions
    )
    {
        services.AddDbContextPool<AppReadDataContext>(
            (
                serviceProvider,
                options
            ) =>
            {
                ConfigureCommonDbContextOptions(
                    options,
                    serviceProvider,
                    connectionString,
                    connectionPoolOptions
                );
            }
        );
    }

    private static void ConfigureCommonDbContextOptions(
        DbContextOptionsBuilder options,
        IServiceProvider serviceProvider,
        string connectionString,
        ConnectionPoolOptions connectionPoolOptions
    )
    {
        var concurrentQueue = serviceProvider.GetRequiredService<ConcurrentQueue<string>>();
        var logger = serviceProvider.GetRequiredService<ILogger<PerformanceInterceptor>>();

        options.UseNpgsql(
                connectionString,
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                    sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                }
            )
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseSnakeCaseNamingConvention()
            .LogTo(message => concurrentQueue.Enqueue(message), LogLevel.Information)
            .EnableDetailedErrors(false)
            .EnableSensitiveDataLogging(false)
            .AddInterceptors(
                new AuditSaveChangesInterceptor(
                    serviceProvider.GetRequiredService<IHttpContextAccessor>()
                ),
                new SemaphoreDbConnectionInterceptor(
                    new SemaphoreSlim(connectionPoolOptions.MaximumNumberOfConcurrentEntries)
                ),
                new PerformanceInterceptor(logger)
            );

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            options.EnableSensitiveDataLogging();
        }
    }

    private static void RegisterDbContextServices(
        IServiceCollection services
    )
    {
        services.AddScoped<AppDataContext>(
            serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<AppDataContext>>();

                DbContextTypeHolder.UseMasterDb.Value = true;
                var useMasterNode = DbContextTypeHolder.UseMasterDb.Value;

                logger.LogInformation(
                    "Using {DbContextType} for database operations.",
                    useMasterNode ? nameof(AppWriteDataContext) : nameof(AppReadDataContext)
                );

                return useMasterNode
                    ? serviceProvider.GetRequiredService<AppWriteDataContext>()
                    : serviceProvider.GetRequiredService<AppReadDataContext>();
            }
        );
        services.AddScoped<DbContext>(
            provider => provider.GetRequiredService<AppDataContext>()
        );
        services.AddSingleton<IDbContextFactory<AppDataContext>, AppDataContextFactory>();
        services.AddDbContextFactory<AppDataContext>();
        services.AddForwardingDbContextFactory<DbContext, AppDataContext>();
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
        var context = scope.ServiceProvider.GetRequiredService<AppDataContext>();

        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        return app;
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        // Base Repositories and Infrastructure
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
        services.AddScoped<IUnitOfWork, AppUnitOfWork>();

        return services;
    }
}
