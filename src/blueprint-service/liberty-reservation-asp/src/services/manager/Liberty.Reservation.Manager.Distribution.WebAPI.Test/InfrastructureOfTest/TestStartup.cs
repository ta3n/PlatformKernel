using Liberty.Reservation.Manager.Application.Contexts;
using Liberty.Reservation.Manager.Application.Contexts.Interceptors;
using Liberty.Reservation.Manager.Application.Domains.Repositories;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;
using Liberty.Reservation.Manager.Distribution.WebAPI.Test.InfrastructureOfTest.Utilities;
using Liberty.UnitOfWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Test.InfrastructureOfTest;

public class TestStartup : Startup.Startup
{
    public override void ConfigureServices(
        IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment
    )
    {
        base.ConfigureServices(services, configuration, environment);

        services.AddScoped<IRoomGroupRepository, RoomGroupRepository>();
    }

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
        AddReservationDatabase(services);
        AddKakusanSettings(services, configuration);
    }

    protected static void AddKakusanSettings(
        IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<C001Setting>(configuration.GetSection("KakusanApiSetting:C001"));
        services.Configure<C002Setting>(configuration.GetSection("KakusanApiSetting:C002"));
        services.Configure<C003Setting>(configuration.GetSection("KakusanApiSetting:C003"));
        services.Configure<C004Setting>(configuration.GetSection("KakusanApiSetting:C004"));
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
                        .EnableDetailedErrors()
                        .AddInterceptors(new CacheSaveChangesInterceptor(serviceProvider));

                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                    {
                        options.EnableSensitiveDataLogging();
                    }
                }
            );

            services.AddTransient(
                _ =>
                {
                    var connection = new NpgsqlConnection(connectionString);
                    var compiler = new PostgresCompiler();
                    return new QueryFactory(connection, compiler);
                }
            );
        }

        services.AddScoped<DbContext>(
            provider => provider.GetRequiredService<ManagerDataContext>()
        );

        services.AddForwardingDbContextFactory<DbContext, ManagerDataContext>();
    }
}
