using System.Collections.Concurrent;
using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Behaviors;
using Liberty.Reservation.Application.Contexts.Interceptors;
using Liberty.Reservation.Application.Domains.Repositories;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.Application;
using Liberty.Reservation.User.Application.Contexts;
using Liberty.Reservation.User.Application.Contexts.Interceptors;
using Liberty.Reservation.User.Application.Domains.Repositories;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork;
using Liberty.UnitOfWork.Abstractions;
using Liberty.UnitOfWork.Implementations;
using Liberty.UnitOfWork.Interceptors;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Liberty.Reservation.User.WebAPI.Configurations;

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
        services.AddDbContextPool<UserWriteDataContext>(
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
        services.AddDbContextPool<UserReadDataContext>(
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
        services.AddScoped<UserDataContext>(
            serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<UserDataContext>>();

                DbContextTypeHolder.UseMasterDb.Value = true;
                var useMasterNode = DbContextTypeHolder.UseMasterDb.Value;

                logger.LogInformation(
                    "Using {DbContextType} for database operations.",
                    useMasterNode ? nameof(UserWriteDataContext) : nameof(UserReadDataContext)
                );

                return useMasterNode
                    ? serviceProvider.GetRequiredService<UserWriteDataContext>()
                    : serviceProvider.GetRequiredService<UserReadDataContext>();
            }
        );
        services.AddScoped<DbContext>(
            provider => provider.GetRequiredService<UserDataContext>()
        );
        services.AddSingleton<IDbContextFactory<UserDataContext>, UserDataContextFactory>();
        services.AddDbContextFactory<UserDataContext>();
        services.AddForwardingDbContextFactory<DbContext, UserDataContext>();
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

        services.AddScoped<IBookingReservationRepository, BookingReservationRepository>();
        services.AddScoped<IBookingDataPlanRepository, BookingDataPlanRepository>();
        services.AddScoped<IBookingRoomGroupRepository, BookingRoomGroupRepository>();
        services.AddScoped<IBookingOptionItemRepository, BookingOptionItemRepository>();
        services.AddScoped<IBookingPersonAgeTypeRepository, BookingPersonAgeTypeRepository>();
        services.AddScoped<IBookingOrderRepository, BookingOrderRepository>();
        services.AddScoped<IBookingOrderReservationRepository, BookingOrderReservationRepository>();
        services.AddScoped<IBookingSiteRepository, BookingSiteRepository>();
        services.AddScoped<IMailTemplateRepository, MailTemplateRepository>();
        services.AddScoped<IBookingFacilityRepository, BookingFacilityRepository>();
        services.AddScoped<IIntegrationEventOutboxRepository, IntegrationEventOutboxRepository>();
        services.AddScoped<ICancellationDataRepository, CancellationDataRepository>();
        services.AddScoped<IDataOfCancellationRepository, DataOfCancellationRepository>();
        services.AddScoped<IRoomGroupAppDateRepository, RoomGroupAppDateRepository>();
        services.AddScoped<IPlanRoomGroupSiteAppDatePriceDataRepository, PlanRoomGroupSiteAppDatePriceDataRepository>();
        services.AddScoped<IFacilityRepository, FacilityRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<IPersonAgeTypeRepository, PersonAgeTypeRepository>();
        services.AddScoped<IOptionItemRepository, OptionItemRepository>();
        services.AddScoped<IOrderGmoPaymentRepository, OrderGmoPaymentRepository>();
        services.AddScoped<ISystemConfigRepository, SystemConfigRepository>();
        services.AddScoped<IBookingSystemConfigRepository, BookingSystemConfigRepository>();
        services.AddScoped<IBookingRoomGroupAppDateRepository, BookingRoomGroupAppDateRepository>();
        services.AddScoped<IBookingDataAppDateRepository, BookingDataAppDateRepository>();
        services.AddScoped<IBookingDataOptionItemRepository, BookingDataOptionItemRepository>();
        services.AddScoped<IBookingDataPersonTypeRepository, BookingDataPersonTypeRepository>();
        services.AddScoped<IBookingDataReservationRepository, BookingDataReservationRepository>();
        services.AddScoped<IBookingDataPriceRepository, BookingDataPriceRepository>();
        services.AddScoped<IBookingDataMetaRepository, BookingDataMetaRepository>();
        services.AddScoped<IOptionItemAppDateRepository, OptionItemAppDateRepository>();
        services.AddScoped<IAlertMessageRepository, AlertMessageRepository>();
        services.AddScoped<IBookingCancellationRepository, BookingCancellationRepository>();

        services.AddScoped<IGmoChangeTranReportRepository, GmoChangeTranReportRepository>();
        return services;
    }
}
