using Liberty.Reservation.Application.Behaviors;
using Liberty.Reservation.Site.Application.Auth;
using Liberty.Reservation.Site.Application.Contexts;
using Liberty.Reservation.Site.Application.Contexts.Interceptors;
using Liberty.Reservation.Site.WebAPI.Application.ExternalServices.Membership.Facility;
using Liberty.Reservation.Site.WebAPI.Configurations;
using Liberty.Reservation.Site.WebAPI.Middlewares;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest.Utilities;
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

namespace Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;

public class TestStartup : Startup.Startup
{
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
            context => context.Request.Headers.ContainsKey(SecurityContextAccessor.FacilityCodeHeaderKey)
                && context.Request.Headers.ContainsKey(SecurityContextAccessor.SiteCodeOfFacilityHeaderKey),
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
        AddReservationDatabase(services);
        AddMembershipFacilityDatabase(services);
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

            services.AddDbContextPool<SiteDataContext>(
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

            services.RemoveAll<DbContextOptions<SiteDataContext>>();
            services.RemoveAll<DbContextOptions<SiteWriteDataContext>>();
            services.RemoveAll<DbContextOptions<SiteReadDataContext>>();

            services.AddDbContextPool<SiteWriteDataContext>(
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
                        .EnableDetailedErrors(false)
                        .EnableSensitiveDataLogging(false)
                        .AddInterceptors(
                            new AuditSaveChangesInterceptor(
                                serviceProvider.GetRequiredService<IHttpContextAccessor>()
                            ),
                            new CacheSaveChangesInterceptor(serviceProvider)
                        );

                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                    {
                        options.EnableSensitiveDataLogging();
                    }
                }
            );

            services.AddDbContextPool<SiteReadDataContext>(
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
                        .EnableDetailedErrors(false)
                        .EnableSensitiveDataLogging(false)
                        .AddInterceptors(
                            new AuditSaveChangesInterceptor(
                                serviceProvider.GetRequiredService<IHttpContextAccessor>()
                            )
                        );

                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                    {
                        options.EnableSensitiveDataLogging();
                    }
                }
            );

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

            services.AddTransient(
                _ =>
                {
                    var connection = new NpgsqlConnection(connectionString);
                    var compiler = new PostgresCompiler();
                    return new QueryFactory(connection, compiler);
                }
            );
        }
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
                        .UseSnakeCaseNamingConvention()
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
