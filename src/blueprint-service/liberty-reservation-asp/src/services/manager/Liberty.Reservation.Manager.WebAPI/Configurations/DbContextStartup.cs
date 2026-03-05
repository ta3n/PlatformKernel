using System.Collections.Concurrent;
using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Behaviors;
using Liberty.Reservation.Application.Contexts.Interceptors;
using Liberty.Reservation.Application.Domains.Repositories;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
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
using Npgsql;
using Serilog;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Liberty.Reservation.Manager.WebAPI.Configurations;

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
        services.AddDbContextPool<ManagerWriteDataContext>(
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
        services.AddDbContextPool<ManagerReadDataContext>(
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
        services.AddScoped<ManagerDataContext>(
            serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<ManagerDataContext>>();

                DbContextTypeHolder.UseMasterDb.Value = true;
                var useMasterNode = DbContextTypeHolder.UseMasterDb.Value;

                logger.LogInformation(
                    "Using {DbContextType} for database operations.",
                    useMasterNode ? nameof(ManagerWriteDataContext) : nameof(ManagerReadDataContext)
                );

                return useMasterNode
                    ? serviceProvider.GetRequiredService<ManagerWriteDataContext>()
                    : serviceProvider.GetRequiredService<ManagerReadDataContext>();
            }
        );
        services.AddScoped<DbContext>(
            provider => provider.GetRequiredService<ManagerDataContext>()
        );
        services.AddSingleton<IDbContextFactory<ManagerDataContext>, ManagerDataContextFactory>();
        services.AddDbContextFactory<ManagerDataContext>();
        services.AddForwardingDbContextFactory<DbContext, ManagerDataContext>();
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
        // Base Repositories and Infrastructure
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
        services.AddScoped<IUnitOfWork, ManagerUnitOfWork>();
        services.AddScoped<ISystemConfigRepository, SystemConfigRepository>();
        services.AddScoped<IIntegrationEventOutboxRepository, IntegrationEventOutboxRepository>();
        services.AddScoped<IMailTemplateRepository, MailTemplateRepository>();

        // Booking Related Repositories
        services.AddScoped<IBookingSystemConfigRepository, BookingSystemConfigRepository>();
        services.AddScoped<IBookingReservationRepository, BookingReservationRepository>();
        services.AddScoped<IBookingDataPlanRepository, BookingDataPlanRepository>();
        services.AddScoped<IBookingRoomGroupRepository, BookingRoomGroupRepository>();
        services.AddScoped<IBookingOptionItemRepository, BookingOptionItemRepository>();
        services.AddScoped<IBookingPersonAgeTypeRepository, BookingPersonAgeTypeRepository>();
        services.AddScoped<IBookingOrderRepository, BookingOrderRepository>();
        services.AddScoped<IBookingOrderReservationRepository, BookingOrderReservationRepository>();
        services.AddScoped<IBookingRoomRepresentativeRepository, BookingRoomRepresentativeRepository>();
        services.AddScoped<IBookingSiteRepository, BookingSiteRepository>();
        services.AddScoped<IBookingFacilityRepository, BookingFacilityRepository>();
        services.AddScoped<IBookingRoomGroupAppDateRepository, BookingRoomGroupAppDateRepository>();
        services.AddScoped<IBookingDataAppDateRepository, BookingDataAppDateRepository>();
        services.AddScoped<IBookingDataOptionItemRepository, BookingDataOptionItemRepository>();
        services.AddScoped<IBookingDataPersonTypeRepository, BookingDataPersonTypeRepository>();
        services.AddScoped<IBookingDataReservationRepository, BookingDataReservationRepository>();
        services.AddScoped<IBookingDataPriceRepository, BookingDataPriceRepository>();
        services.AddScoped<IBookingDataMetaRepository, BookingDataMetaRepository>();

        // Facility Related Repositories
        services.AddScoped<IFacilityRepository, FacilityRepository>();
        services.AddScoped<IFacilityOptionItemRepository, FacilityOptionItemRepository>();
        services.AddScoped<IFacilityRoomGroupRepository, FacilityRoomGroupRepository>();
        services.AddScoped<IFacilityPersonAgeTypeRepository, FacilityPersonAgeTypeRepository>();
        services.AddScoped<IFacilityFileRepository, FacilityFileRepository>();
        services.AddScoped<IFacilityQuestionRepository, FacilityQuestionRepository>();
        services.AddScoped<IFacilityPlanRepository, FacilityPlanRepository>();
        services.AddScoped<IFacilityCategoryRepository, FacilityCategoryRepository>();
        services.AddScoped<IFacilityCalendarRepository, FacilityCalendarRepository>();
        services.AddScoped<IFacilityAppDateTypeRepository, FacilityAppDateTypeRepository>();
        services.AddScoped<IFacilityCancellationRepository, FacilityCancellationRepository>();
        services.AddScoped<IFacilitySiteRepository, FacilitySiteRepository>();
        services.AddScoped<IFacilityAllergenRepository, FacilityAllergenRepository>();

        // Room Group Related Repositories
        services.AddScoped<IRoomGroupRepository, RoomGroupRepository>();
        services.AddScoped<IRoomGroupBedTypeRepository, RoomGroupBedTypeRepository>();
        services.AddScoped<IRoomGroupCategoryRepository, RoomGroupCategoryRepository>();
        services.AddScoped<IRoomGroupSiteRepository, RoomGroupSiteRepository>();
        services.AddScoped<IRoomGroupAppDateRepository, RoomGroupAppDateRepository>();
        services.AddScoped<IFileRoomGroupRepository, FileRoomGroupRepository>();

        // Option Item Related Repositories
        services.AddScoped<IOptionItemRepository, OptionItemRepository>();
        services.AddScoped<IOptionItemCategoryRepository, OptionItemCategoryRepository>();
        services.AddScoped<IOptionItemQuestionRepository, OptionItemQuestionRepository>();
        services.AddScoped<IOptionItemAppDateRepository, OptionItemAppDateRepository>();
        services.AddScoped<IFileOptionItemRepository, FileOptionItemRepository>();

        // Calendar and Date Related Repositories
        services.AddScoped<ICalendarRepository, CalendarRepository>();
        services.AddScoped<IAppDateRepository, AppDateRepository>();
        services.AddScoped<IAppDateTypeRepository, AppDateTypeRepository>();
        services.AddScoped<ICalendarAppDateAppDateTypeRepository, CalendarAppDateAppDateTypeRepository>();

        // Plan Related Repositories
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<IPlanRoomGroupSiteAppDateRepository, PlanRoomGroupSiteAppDateRepository>();
        services.AddScoped<IPlanRoomGroupSiteAppDatePriceDataRepository, PlanRoomGroupSiteAppDatePriceDataRepository>();
        services.AddScoped<IPlanRoomGroupSiteDiscountDataRepository, PlanRoomGroupSiteDiscountDataRepository>();
        services.AddScoped<IPlanRoomGroupSitePersonAgeTypeRepository, PlanRoomGroupSitePersonAgeTypeRepository>();
        services.AddScoped<IPlanRoomGroupSiteRepository, PlanRoomGroupSiteRepository>();
        services.AddScoped<IPlanRoomGroupRepository, PlanRoomGroupRepository>();
        services.AddScoped<IPlanRoomGroupSiteAppDateTypePriceDataRepository, PlanRoomGroupSiteAppDateTypePriceDataRepository>();
        services.AddScoped<IPlanCategoryRepository, PlanCategoryRepository>();
        services.AddScoped<IPlanOptionItemRepository, PlanOptionItemRepository>();
        services.AddScoped<IPlanQuestionRepository, PlanQuestionRepository>();
        services.AddScoped<IPlanMealTypeRepository, PlanMealTypeRepository>();
        services.AddScoped<IFilePlanRepository, FilePlanRepository>();
        services.AddScoped<IPlanSiteRepository, PlanSiteRepository>();

        // Cancellation Related Repositories
        services.AddScoped<ICancellationRepository, CancellationRepository>();
        services.AddScoped<ICancellationDataRepository, CancellationDataRepository>();
        services.AddScoped<IDataOfCancellationRepository, DataOfCancellationRepository>();

        // Customer Related Repositories
        services.AddScoped<ICustomerInfoRepository, CustomerInfoRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IReservationRoomGroupAppDatePersonAgeTypeRepository, ReservationRoomGroupAppDatePersonAgeTypeRepository>();

        // Misc Repositories
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IFileCategoryRepository, FileCategoryRepository>();
        services.AddScoped<IAreaRepository, AreaRepository>();
        services.AddScoped<IBedTypeRepository, BedTypeRepository>();
        services.AddScoped<ISiteRepository, SiteRepository>();
        services.AddScoped<IAllergenRepository, AllergenRepository>();
        services.AddScoped<IMealTypeRepository, MealTypeRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IPersonAgeTypeRepository, PersonAgeTypeRepository>();
        services.AddScoped<IPersonAgeTypeSpaTaxDataRepository, PersonAgeTypeSpaTaxDataRepository>();
        services.AddScoped<IBookingCancellationRepository, BookingCancellationRepository>();

        services.AddScoped<IAlertMessageRepository, AlertMessageRepository>();
        services.AddScoped<IGmoChangeTranReportRepository, GmoChangeTranReportRepository>();

        return services;
    }
}
