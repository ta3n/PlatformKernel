using System.Collections.Concurrent;
using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Behaviors;
using Liberty.Reservation.Application.Contexts.Interceptors;
using Liberty.Reservation.Application.Domains.Repositories;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application;
using Liberty.Reservation.Employee.Application.Contexts;
using Liberty.Reservation.Employee.Application.Contexts.Interceptors;
using Liberty.Reservation.Employee.Application.Domains.Repositories;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork;
using Liberty.UnitOfWork.Abstractions;
using Liberty.UnitOfWork.Implementations;
using Liberty.UnitOfWork.Interceptors;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Liberty.Reservation.Employee.WebAPI.Configurations;

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
                return new QueryFactory(connection, compiler);
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
        services.AddDbContextPool<EmployeeWriteDataContext>(
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
        services.AddDbContextPool<EmployeeReadDataContext>(
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
            .EnableDetailedErrors()
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
        services.AddScoped<EmployeeDataContext>(
            serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<EmployeeDataContext>>();

                DbContextTypeHolder.UseMasterDb.Value = true;
                var useMasterNode = DbContextTypeHolder.UseMasterDb.Value;

                logger.LogInformation(
                    "Using {DbContextType} for database operations.",
                    useMasterNode ? nameof(EmployeeWriteDataContext) : nameof(EmployeeReadDataContext)
                );

                return useMasterNode
                    ? serviceProvider.GetRequiredService<EmployeeWriteDataContext>()
                    : serviceProvider.GetRequiredService<EmployeeReadDataContext>();
            }
        );
        services.AddScoped<DbContext>(
            provider => provider.GetRequiredService<EmployeeDataContext>()
        );
        services.AddSingleton<IDbContextFactory<EmployeeDataContext>, EmployeeDataContextFactory>();
        services.AddDbContextFactory<EmployeeDataContext>();
        services.AddForwardingDbContextFactory<DbContext, EmployeeDataContext>();
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
        var context = scope.ServiceProvider.GetRequiredService<EmployeeDataContext>();

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

        services.AddScoped<IUnitOfWork, EmployeeUnitOfWork>();

        services.AddScoped<ILanguageRepository, LanguageRepository>();
        services.AddScoped<IFacilityLanguageRepository, FacilityLanguageRepository>();
        services.AddScoped<IAppDateRepository, AppDateRepository>();
        services.AddScoped<IAppDateTypeRepository, AppDateTypeRepository>();
        services.AddScoped<IAppDateDataRepository, AppDateDataRepository>();
        services.AddScoped<IDateDataOfAppDateRepository, DateDataOfAppDateRepository>();
        services.AddScoped<IDateTypeOfAppDateRepository, DateTypeOfAppDateRepository>();
        services.AddScoped<IFacilityRepository, FacilityRepository>();
        services.AddScoped<ISiteRepository, SiteRepository>();
        services.AddScoped<IAreaRepository, AreaRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ISystemConfigRepository, SystemConfigRepository>();
        services.AddScoped<IConsumptionTaxRepository, ConsumptionTaxRepository>();
        services.AddScoped<IFaxServiceRepository, FaxServiceRepository>();
        services.AddScoped<IFacilitySiteRepository, FacilitySiteRepository>();
        services.AddScoped<IFacilityFaxSrvRepository, FacilityFaxSrvRepository>();
        services.AddScoped<IFacilityAppDateTypeRepository, FacilityAppDateTypeRepository>();
        services.AddScoped<IFacilityAllergenRepository, FacilityAllergenRepository>();
        services.AddScoped<IFacilityPersonAgeTypeRepository, FacilityPersonAgeTypeRepository>();
        services.AddScoped<IPersonAgeTypeRepository, PersonAgeTypeRepository>();
        services.AddScoped<IAllergenRepository, AllergenRepository>();
        services.AddScoped<IPersonAgeTypeSpaTaxDataRepository, PersonAgeTypeSpaTaxDataRepository>();
        services.AddScoped<IFacilityCategoryRepository, FacilityCategoryRepository>();
        services.AddScoped<IPlanCategoryRepository, PlanCategoryRepository>();
        services.AddScoped<IRoomGroupCategoryRepository, RoomGroupCategoryRepository>();
        services.AddScoped<IOptionItemCategoryRepository, OptionItemCategoryRepository>();
        services.AddScoped<IFileCategoryRepository, FileCategoryRepository>();
        services.AddScoped<ISpaTaxDataRepository, SpaTaxDataRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IBookingReservationRepository, BookingReservationRepository>();
        services.AddScoped<IMailTemplateRepository, MailTemplateRepository>();
        services.AddScoped<IAlertMessageRepository, AlertMessageRepository>();
        services.AddScoped<IBookingCancellationRepository, BookingCancellationRepository>();

        return services;
    }
}
