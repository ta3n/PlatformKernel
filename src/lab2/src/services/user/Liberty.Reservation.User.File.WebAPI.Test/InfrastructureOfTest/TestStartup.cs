using Liberty.Reservation.User.Application.Contexts;
using Liberty.Reservation.User.Application.Contexts.Interceptors;
using Liberty.Reservation.User.File.WebAPI.Test.InfrastructureOfTest.Utilities;
using Liberty.UnitOfWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Liberty.Reservation.User.File.WebAPI.Test.InfrastructureOfTest;

public class TestStartup : Startup.Startup
{
    public override void ConfigureMiddleware(
        IApplicationBuilder app,
        IHostEnvironment environment,
        IConfiguration configuration
    )
    {
        environment.EnvironmentName = Environments.Development;
        base.ConfigureMiddleware(app, environment, configuration);
    }

    protected override void AddDatabase(
        IConfiguration configuration,
        IServiceCollection services
    )
    {
        if (TestUtil.IsUseTestSqlite())
        {
            var connection = new SqliteConnection(
                new SqliteConnectionStringBuilder { DataSource = ":memory:" }.ToString()
            );

            services.AddDbContextPool<UserDataContext>(
                context => context.UseSqlite(connection)
            );
        }
        else
        {
            var connectionString = MockPostgreSqlContainer.ReservationConnectionString;

            services.RemoveAll<DbContextOptions<UserDataContext>>();

            services.AddDbContextPool<UserDataContext>(
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
            provider => provider.GetRequiredService<UserDataContext>()
        );

        services.AddForwardingDbContextFactory<DbContext, UserDataContext>();
    }
}
