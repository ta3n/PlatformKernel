using System.Collections.Concurrent;
using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Contexts.Interceptors;
using Liberty.Reservation.Application.Domains.Repositories;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application;
using Liberty.Reservation.Manager.Application.Auth;
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
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Liberty.Reservation.Booking.Worker.Configurations;

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

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<ISecurityContextAccessor, SecurityContextAccessor>();
        services.AddHttpContextAccessor();
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

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));

        services.AddScoped<IUnitOfWork, ManagerUnitOfWork>();

        services.AddScoped<ISystemConfigRepository, SystemConfigRepository>();
        services.AddScoped<IFacilityRepository, FacilityRepository>();
        services.AddScoped<IBookingReservationRepository, BookingReservationRepository>();
        services.AddScoped<IMailTemplateRepository, MailTemplateRepository>();
        services.AddScoped<IIntegrationEventOutboxRepository, IntegrationEventOutboxRepository>();
        services.AddScoped<IRoomGroupAppDateRepository, RoomGroupAppDateRepository>();
        services.AddScoped<IAdjustmentResultRepository, AdjustmentResultRepository>();
        services.AddScoped<IRoomAdjustmentStatusRepository, RoomAdjustmentStatusRepository>();
        services.AddScoped<IAppDateRepository, AppDateRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IBookingAggregateAuditLogRepository, BookingAggregateAuditLogRepository>();
        services.AddScoped<IBookingAggregateMailAuditLogRepository, BookingAggregateMailAuditLogRepository>();
        services.AddScoped<IBookingAggregateFaxAuditLogRepository, BookingAggregateFaxAuditLogRepository>();

        services.AddScoped<IBookingQuestionRepository, BookingQuestionRepository>();
        services.AddScoped<IBookingOptionItemRepository, BookingOptionItemRepository>();
        services.AddScoped<IBookingPersonAgeTypeRepository, BookingPersonAgeTypeRepository>();
        services.AddScoped<IBookingFacilityRepository, BookingFacilityRepository>();

        return services;
    }
}
