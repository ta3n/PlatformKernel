using Liberty.Reservation.Site.Application.Contexts;
using Liberty.Reservation.Site.Application.Contexts.Interceptors;
using Liberty.Reservation.Site.File.WebAPI.Application.ExternalServices.Membership.Facility;
using Liberty.Reservation.Site.File.WebAPI.Test.InfrastructureOfTest.Utilities;
using Liberty.UnitOfWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Site.File.WebAPI.Test.InfrastructureOfTest;

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
        base.ConfigureMiddleware(app, environment, configuration);
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
        }
        else
        {
            var connectionString = MockPostgreSqlContainer.ReservationConnectionString;

            services.RemoveAll<DbContextOptions<SiteDataContext>>();

            services.AddDbContextPool<SiteDataContext>(
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
                            }
                        )
                        .UseSnakeCaseNamingConvention()
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

        services.AddScoped<DbContext>(
            provider => provider.GetRequiredService<SiteDataContext>()
        );

        services.AddForwardingDbContextFactory<DbContext, SiteDataContext>();
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
