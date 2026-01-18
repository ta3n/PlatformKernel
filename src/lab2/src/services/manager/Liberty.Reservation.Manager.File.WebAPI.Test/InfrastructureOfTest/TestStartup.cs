using Liberty.Reservation.Manager.Application.Contexts;
using Liberty.Reservation.Manager.Application.Contexts.Interceptors;
using Liberty.Reservation.Manager.File.WebAPI.Application.ExternalServices.Membership.Facility;
using Liberty.Reservation.Manager.File.WebAPI.Test.InfrastructureOfTest.Utilities;
using Liberty.UnitOfWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Manager.File.WebAPI.Test.InfrastructureOfTest;

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
        AddMembershipFacilityDatabase(services);
        AddReservationDatabase(services);
    }

    protected static void AddReservationDatabase(
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
        }
        else
        {
            var connectionString = MockPostgreSqlContainer.ReservationConnectionString;

            services.RemoveAll<DbContextOptions<ManagerDataContext>>();

            services.AddDbContextPool<ManagerDataContext>(
                (
                    serviceProvider,
                    options
                ) =>
                {
                    options.UseNpgsql(
                            connectionString,
                            sqlOptions =>
                            {
                                sqlOptions.SetPostgresVersion(12, 0);
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
            provider => provider.GetRequiredService<ManagerDataContext>()
        );

        services.AddForwardingDbContextFactory<DbContext, ManagerDataContext>();
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
            var membershipConnectionString =
                MockPostgreSqlContainer.FacilityConnectionString;
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
