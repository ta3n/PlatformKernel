using System.Collections.Concurrent;
using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Behaviors;
using Liberty.Reservation.Application.Contexts.Interceptors;
using Liberty.Reservation.Application.Domains.Repositories;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Site.Application;
using Liberty.Reservation.Site.Application.Contexts;
using Liberty.Reservation.Site.Application.Contexts.Interceptors;
using Liberty.Reservation.Site.Application.Domains.Repositories;
using Liberty.Reservation.Site.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork;
using Liberty.UnitOfWork.Abstractions;
using Liberty.UnitOfWork.Implementations;
using Liberty.UnitOfWork.Interceptors;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Liberty.Reservation.Site.WebAPI.Configurations;

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
        services.AddDbContextPool<SiteWriteDataContext>(
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
            },
            128
        );
    }

    private static void ConfigureReadDataContext(
        IServiceCollection services,
        string connectionString,
        ConnectionPoolOptions connectionPoolOptions
    )
    {
        services.AddDbContextPool<SiteReadDataContext>(
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
            },
            128
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
        services.AddScoped<SiteDataContext>(
            serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<SiteDataContext>>();

                DbContextTypeHolder.UseMasterDb.Value = true;
                var useMasterNode = DbContextTypeHolder.UseMasterDb.Value;

                logger.LogInformation(
                    "Using {DbContextType} for database operations.",
                    useMasterNode ? nameof(SiteWriteDataContext) : nameof(SiteReadDataContext)
                );

                return useMasterNode
                    ? serviceProvider.GetRequiredService<SiteWriteDataContext>()
                    : serviceProvider.GetRequiredService<SiteReadDataContext>();
            }
        );
        services.AddScoped<DbContext>(
            provider => provider.GetRequiredService<SiteDataContext>()
        );
        services.AddSingleton<IDbContextFactory<SiteDataContext>, SiteDataContextFactory>();
        services.AddDbContextFactory<SiteDataContext>();
        services.AddForwardingDbContextFactory<DbContext, SiteDataContext>();
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
        var context = scope.ServiceProvider.GetRequiredService<SiteDataContext>();

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

        services.AddScoped<IUnitOfWork, SiteUnitOfWork>();

        services.AddScoped<IMailTemplateRepository, MailTemplateRepository>();
        services.AddScoped<IBookingReservationRepository, BookingReservationRepository>();
        services.AddScoped<IBookingRoomGroupRepository, BookingRoomGroupRepository>();
        services.AddScoped<IBookingOptionItemRepository, BookingOptionItemRepository>();
        services.AddScoped<IBookingPersonAgeTypeRepository, BookingPersonAgeTypeRepository>();
        services.AddScoped<IBookingOrderRepository, BookingOrderRepository>();
        services.AddScoped<IBookingOrderReservationRepository, BookingOrderReservationRepository>();
        services.AddScoped<IBookingRoomRepresentativeRepository, BookingRoomRepresentativeRepository>();
        services.AddScoped<IBookingSiteRepository, BookingSiteRepository>();
        services.AddScoped<IBookingFacilityRepository, BookingFacilityRepository>();
        services.AddScoped<IBookingRoomGroupAppDateRepository, BookingRoomGroupAppDateRepository>();
        services.AddScoped<IIntegrationEventOutboxRepository, IntegrationEventOutboxRepository>();
        services.AddScoped<IRoomGroupAppDateRepository, RoomGroupAppDateRepository>();
        services.AddScoped<IPlanRoomGroupSiteAppDatePriceDataRepository, PlanRoomGroupSiteAppDatePriceDataRepository>();
        services.AddScoped<IFacilityRepository, FacilityRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IFilePlanRepository, FilePlanRepository>();
        services.AddScoped<IPlanCategoryRepository, PlanCategoryRepository>();
        services.AddScoped<IPlanMealTypeRepository, PlanMealTypeRepository>();
        services.AddScoped<IBookingPlanOptionItemRepository, BookingPlanOptionItemRepository>();
        services.AddScoped<IPlanQuestionRepository, PlanQuestionRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<IPlanRoomGroupRepository, PlanRoomGroupRepository>();
        services.AddScoped<IBookingPlanRoomGroupSiteAppDateRepository, BookingPlanRoomGroupSiteAppDateRepository>();
        services
            .AddScoped<IBookingPlanRoomGroupSiteAppDateTypePriceDataRepository, BookingPlanRoomGroupSiteAppDateTypePriceDataRepository>();
        services.AddScoped<IBookingPlanRoomGroupSiteDiscountDataRepository, BookingPlanRoomGroupSiteDiscountDataRepository>();
        services.AddScoped<IBookingPlanRoomGroupSitePersonAgeTypeRepository, BookingPlanRoomGroupSitePersonAgeTypeRepository>();
        services.AddScoped<IPlanRoomGroupSiteRepository, PlanRoomGroupSiteRepository>();
        services.AddScoped<IBookingReservationPlanRoomGroupAppDateRepository, BookingReservationPlanRoomGroupAppDateRepository>();
        services.AddScoped<IRoomGroupRepository, RoomGroupRepository>();
        services.AddScoped<ISiteRepository, SiteRepository>();
        services.AddScoped<IFacilityPlanRepository, FacilityPlanRepository>();
        services.AddScoped<IReservationQuestionRepository, ReservationQuestionRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IReservationRoomGroupAppDateOptionItemRepository, ReservationRoomGroupAppDateOptionItemRepository>();
        services.AddScoped<IReservationRoomGroupAppDatePersonAgeTypeRepository, ReservationRoomGroupAppDatePersonAgeTypeRepository>();
        services.AddScoped<ISystemConfigRepository, SystemConfigRepository>();
        services.AddScoped<IOptionItemRepository, OptionItemRepository>();
        services.AddScoped<IFacilitySiteRepository, FacilitySiteRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IFacilityPersonAgeTypeRepository, FacilityPersonAgeTypeRepository>();
        services.AddScoped<IFacilityRoomGroupRepository, FacilityRoomGroupRepository>();
        services.AddScoped<IBookingSystemConfigRepository, BookingSystemConfigRepository>();
        services.AddScoped<IBookingFacilityRepository, BookingFacilityRepository>();
        services.AddScoped<IBookingDataPlanRepository, BookingDataPlanRepository>();
        services.AddScoped<IBookingDataAppDateRepository, BookingDataAppDateRepository>();
        services.AddScoped<IBookingDataOptionItemRepository, BookingDataOptionItemRepository>();
        services.AddScoped<IBookingDataPersonTypeRepository, BookingDataPersonTypeRepository>();
        services.AddScoped<IBookingDataReservationRepository, BookingDataReservationRepository>();
        services.AddScoped<IBookingDataPriceRepository, BookingDataPriceRepository>();
        services.AddScoped<IBookingDataMetaRepository, BookingDataMetaRepository>();
        services.AddScoped<IAlertMessageRepository, AlertMessageRepository>();
        services.AddScoped<IBookingCancellationRepository, BookingCancellationRepository>();

        return services;
    }
}
