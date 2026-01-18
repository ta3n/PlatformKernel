using System.Data;
using Liberty.GmoPaymentGateway.Options;
using Liberty.Reservation.Application.Behaviors;
using Liberty.Reservation.Application.Contexts.Interceptors;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Contexts;
using Liberty.Reservation.Manager.Application.Contexts.Interceptors;
using Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility;
using Liberty.Reservation.Manager.WebAPI.Application.HostedServices;
using Liberty.Reservation.Manager.WebAPI.Configurations;
using Liberty.Reservation.Manager.WebAPI.Initializations;
using Liberty.Reservation.Manager.WebAPI.Middlewares;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using Liberty.UnitOfWork;
using Liberty.UnitOfWork.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;

public class TestStartup : Startup.Startup
{
    public override void ConfigureServices(
        IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment
    )
    {
        base.ConfigureServices(services, configuration, environment);

        services.Configure<GmoPaymentOptions>(
            options =>
            {
                options.Enabled = true;
                options.Url = "https://pt01.mul-pay.jp/link/tshop00035518/Multi/Entry";
                options.PaymentHost = "https://pt01.mul-pay.jp/payment";
                options.ShopId = "tshop00035518";
                options.ShopPassword = "dcnycrv1";
                options.JobCd = "CAPTURE";
                options.OrderDateFormat = "yyyyMMddHHmmss";
                options.UseCredit = 1;
                options.Expire = 60;
            }
        );
    }

    public override void ConfigureMiddleware(
        IApplicationBuilder app,
        IHostEnvironment environment,
        IConfiguration configuration
    )
    {
        environment.EnvironmentName = Environments.Development;
        app.UseMembershipDatabase(environment);

        app
            .UseApplicationDatabase(environment)
            .UseApplicationSecurity(configuration)
            .UseApplicationProblemDetails(environment);

        app.UseWhen(
            context => context.Request.Headers.ContainsKey(SecurityContextAccessor.FacilityHeaderKey),
            appBuilder =>
            {
                appBuilder.UseMiddleware<CheckFacilityAvailableMiddleware>();
            }
        );
    }

    protected override void AddDatabase(
        IConfiguration configuration,
        IServiceCollection services
    )
    {
        AddMembershipFacilityDatabase(services);
        AddReservationDatabase(services);
        services.AddHostedService<InitializationService>();
        services.AddHostedService<IntegrationEventOutboxHostedService>();
    }

    private static void AddReservationDatabase(
        IServiceCollection services
    )
    {
        if (TestUtil.IsUseTestSqlite())
        {
            var connection = new SqliteConnection(
                new SqliteConnectionStringBuilder { DataSource = ":memory:" }.ToString()
            );

            services.AddDbContextPool<ManagerDataContext>(
                context => context.UseSqlite(connection)
            );

            services.AddTransient(
                _ =>
                {
                    var compiler = new PostgresCompiler();
                    return new QueryFactory(connection, compiler);
                }
            );
        }
        else
        {
            var connectionString = MockPostgreSqlContainer.ReservationConnectionString;
            var connectionReadDataString = string.Empty;

            services.RemoveAll<DbContextOptions<ManagerDataContext>>();
            services.RemoveAll<DbContextOptions<ManagerWriteDataContext>>();
            services.RemoveAll<DbContextOptions<ManagerReadDataContext>>();

            services.AddDbContextPool<ManagerWriteDataContext>(
                (
                    serviceProvider,
                    options
                ) =>
                {
                    options.UseNpgsql(
                            connectionString,
                            sqlOptions =>
                            {
                                sqlOptions.EnableRetryOnFailure(
                                    5,
                                    TimeSpan.FromSeconds(30),
                                    null
                                );
                                sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                            }
                        )
                        .UseSnakeCaseNamingConvention()
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                        .EnableDetailedErrors(false)
                        .EnableSensitiveDataLogging(false)
                        .AddInterceptors(
                            new DbContextPoolingInterceptor(
                                serviceProvider.GetRequiredService<ILogger<DbContextPoolingInterceptor>>()
                            ),
                            new AuditSaveChangesInterceptor(
                                serviceProvider.GetRequiredService<IHttpContextAccessor>()
                            ),
                            new SemaphoreDbConnectionInterceptor(
                                new SemaphoreSlim(20)
                            ),
                            // new CacheSaveChangesInterceptor(serviceProvider),
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

            services.AddDbContextPool<ManagerReadDataContext>(
                (
                    serviceProvider,
                    options
                ) =>
                {
                    options.UseNpgsql(
                            connectionReadDataString,
                            sqlOptions =>
                            {
                                sqlOptions.EnableRetryOnFailure(
                                    5,
                                    TimeSpan.FromSeconds(30),
                                    null
                                );
                                sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                            }
                        )
                        .UseSnakeCaseNamingConvention()
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                        .EnableDetailedErrors(false)
                        .EnableSensitiveDataLogging(false)
                        .AddInterceptors(
                            new DbContextPoolingInterceptor(
                                serviceProvider.GetRequiredService<ILogger<DbContextPoolingInterceptor>>()
                            ),
                            new AuditSaveChangesInterceptor(
                                serviceProvider.GetRequiredService<IHttpContextAccessor>()
                            ),
                            new SemaphoreDbConnectionInterceptor(
                                new SemaphoreSlim(20)
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

            services.AddTransient(
                _ =>
                {
                    var connection = new NpgsqlConnection(connectionString);
                    var compiler = new PostgresCompiler();
                    return new QueryFactory(connection, compiler);
                }
            );
            services.AddScoped<IDbConnection>(_ =>
            {
                var conn = new NpgsqlConnection(connectionString);
                return conn;
            });
        }

        services.AddScoped<DbContext>(
            provider => provider.GetRequiredService<ManagerDataContext>()
        );

        services.AddForwardingDbContextFactory<DbContext, ManagerDataContext>();

        services.AddDbContextFactory<ManagerDataContext>();
    }

    private static void AddMembershipFacilityDatabase(
        IServiceCollection services
    )
    {
        if (TestUtil.IsUseTestSqlite())
        {
            var connection = new SqliteConnection(
                new SqliteConnectionStringBuilder { DataSource = ":memory:" }.ToString()
            );

            services.AddDbContextPool<MembershipFacilityExternalDbContext>(
                context => context.UseSqlite(connection)
            );
        }
        else
        {
            var membershipConnectionString = MockPostgreSqlContainer.FacilityConnectionString;
            services.RemoveAll<DbContextOptions<MembershipFacilityExternalDbContext>>();

            services.AddDbContextPool<MembershipFacilityExternalDbContext>(
                (
                    serviceProvider,
                    options
                ) =>
                {
                    options
                        .UseNpgsql(
                            membershipConnectionString,
                            sqlOptions =>
                            {
                                sqlOptions.EnableRetryOnFailure(
                                    5,
                                    TimeSpan.FromSeconds(30),
                                    null
                                );
                            }
                        )
                        .LogTo(Console.WriteLine, LogLevel.Information)
                        .EnableDetailedErrors(false)
                        .EnableSensitiveDataLogging(false)
                        .AddInterceptors(new CacheSaveChangesInterceptor(serviceProvider));

                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                    {
                        options.EnableSensitiveDataLogging();
                    }
                }
            );
        }
    }
}
