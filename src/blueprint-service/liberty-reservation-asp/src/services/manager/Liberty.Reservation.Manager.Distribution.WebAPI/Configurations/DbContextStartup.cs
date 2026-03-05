using System.Collections.Concurrent;
using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Contexts.Interceptors;
using Liberty.Reservation.Application.Domains.Repositories;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Manager.Application;
using Liberty.Reservation.Manager.Application.Contexts;
using Liberty.Reservation.Manager.Application.Domains.Repositories;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork;
using Liberty.UnitOfWork.Abstractions;
using Liberty.UnitOfWork.Implementations;
using Liberty.UnitOfWork.Interceptors;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Configurations;

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
                    .EnableDetailedErrors()
                    .AddInterceptors(
                        new AuditSaveChangesInterceptor(
                            serviceProvider.GetRequiredService<IHttpContextAccessor>()
                        ),
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
            provider => provider.GetRequiredService<ManagerDataContext>()
        );
        services.AddDbContextFactory<ManagerDataContext>();
        services.AddForwardingDbContextFactory<DbContext, ManagerDataContext>();

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
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IRoomGroupAppDateRepository, RoomGroupAppDateRepository>();
        services.AddScoped<IFacilityRoomGroupRepository, FacilityRoomGroupRepository>();
        services.AddScoped<IBookingSystemConfigRepository, BookingSystemConfigRepository>();
        services.AddScoped<IBookingFacilityRepository, BookingFacilityRepository>();
        services.AddScoped<IBookingPlanRoomGroupSiteAppDateRepository, BookingPlanRoomGroupSiteAppDateRepository>();
        services
            .AddScoped<IBookingPlanRoomGroupSiteAppDateTypePriceDataRepository, BookingPlanRoomGroupSiteAppDateTypePriceDataRepository>();
        services.AddScoped<IBookingPlanRoomGroupSiteDiscountDataRepository, BookingPlanRoomGroupSiteDiscountDataRepository>();
        services.AddScoped<IBookingPlanRoomGroupSitePersonAgeTypeRepository, BookingPlanRoomGroupSitePersonAgeTypeRepository>();
        services.AddScoped<IBookingPlanOptionItemRepository, BookingPlanOptionItemRepository>();
        services.AddScoped<IBookingReservationPlanRoomGroupAppDateRepository, BookingReservationPlanRoomGroupAppDateRepository>();
        services.AddScoped<IFacilityPlanRepository, FacilityPlanRepository>();
        services.AddScoped<IFacilitySiteRepository, FacilitySiteRepository>();
        services.AddScoped<IPlanRoomGroupSiteAppDatePriceDataRepository, PlanRoomGroupSiteAppDatePriceDataRepository>();
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
        services.AddScoped<IMailTemplateRepository, MailTemplateRepository>();
        services.AddScoped<IFacilityPersonAgeTypeRepository, FacilityPersonAgeTypeRepository>();
        services.AddScoped<IRoomGroupRepository, RoomGroupRepository>();
        services.AddScoped<IAppDateRepository, AppDateRepository>();
        services.AddScoped<IBookingDataPlanRepository, BookingDataPlanRepository>();
        services.AddScoped<IBookingDataAppDateRepository, BookingDataAppDateRepository>();
        services.AddScoped<IBookingDataOptionItemRepository, BookingDataOptionItemRepository>();
        services.AddScoped<IBookingDataPersonTypeRepository, BookingDataPersonTypeRepository>();
        services.AddScoped<IBookingDataReservationRepository, BookingDataReservationRepository>();
        services.AddScoped<IBookingDataPriceRepository, BookingDataPriceRepository>();
        services.AddScoped<IBookingDataMetaRepository, BookingDataMetaRepository>();
        services.AddScoped<IPlanRoomGroupSiteAppDateTypePriceDataRepository, PlanRoomGroupSiteAppDateTypePriceDataRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<IPlanRoomGroupSiteAppDatePriceDataRepository, PlanRoomGroupSiteAppDatePriceDataRepository>();
        services.AddScoped<IPlanRoomGroupRepository, PlanRoomGroupRepository>();
        services.AddScoped<IFilePlanRepository, FilePlanRepository>();
        services.AddScoped<IPlanRoomGroupSiteAppDateRepository, PlanRoomGroupSiteAppDateRepository>();
        services.AddScoped<IPlanRoomGroupSiteRepository, PlanRoomGroupSiteRepository>();
        services.AddScoped<IRoomGroupCategoryRepository, RoomGroupCategoryRepository>();
        services.AddScoped<IRoomGroupSiteRepository, RoomGroupSiteRepository>();
        services.AddScoped<IRoomGroupBedTypeRepository, RoomGroupBedTypeRepository>();
        services.AddScoped<ISiteRepository, SiteRepository>();
        services.AddScoped<IAdjustmentResultRepository, AdjustmentResultRepository>();
        services.AddScoped<IRoomAdjustmentStatusRepository, RoomAdjustmentStatusRepository>();

        return services;
    }
}
