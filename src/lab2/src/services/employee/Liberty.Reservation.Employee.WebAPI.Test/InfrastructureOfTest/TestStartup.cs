using Liberty.Reservation.Application.Behaviors;
using Liberty.Reservation.Employee.Application.Contexts;
using Liberty.Reservation.Employee.Application.Contexts.Interceptors;
using Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility;
using Liberty.Reservation.Employee.WebAPI.Configurations;
using Liberty.Reservation.Employee.WebAPI.Initializations;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;
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

namespace Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

public class TestStartup : Startup.Startup
{
    public override void ConfigureMiddleware(
        IApplicationBuilder app,
        IHostEnvironment environment,
        IConfiguration configuration
    )
    {
        environment.EnvironmentName = Environments.Development;

        app
            .UseResponseCaching()
            .UseApplicationDatabase(environment)
            .UseApplicationSecurity(configuration)
            .UseApplicationProblemDetails(environment);
    }

    protected override void AddDatabase(
        IConfiguration configuration,
        IServiceCollection services
    )
    {
        AddReservationDatabase(services);
        AddMembershipFacilityDatabase(services);
        services.AddHostedService<InitializationService>();
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

            services.AddDbContextPool<EmployeeDataContext>(context => context.UseSqlite(connection));
        }
        else
        {
            var connectionString = MockPostgreSqlContainer.ReservationConnectionString;
            var connectionReadDataString = string.Empty;

            services.RemoveAll<DbContextOptions<EmployeeDataContext>>();
            services.RemoveAll<DbContextOptions<EmployeeWriteDataContext>>();
            services.RemoveAll<DbContextOptions<EmployeeReadDataContext>>();

            services.AddDbContextPool<EmployeeWriteDataContext>(
                (
                    serviceProvider,
                    options
                ) =>
                {
                    options
                        .UseNpgsql(
                            connectionString,
                            sqlOptions =>
                            {
                                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
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

            services.AddDbContextPool<EmployeeReadDataContext>(
                (
                    serviceProvider,
                    options
                ) =>
                {
                    options
                        .UseNpgsql(
                            connectionReadDataString,
                            sqlOptions =>
                            {
                                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
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
        }

        services.AddScoped<EmployeeDataContext>(
            serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<EmployeeDataContext>>();

                DbContextTypeHolder.UseMasterDb.Value = true;
                var useMasterNode = DbContextTypeHolder.UseMasterDb.Value;

                logger.LogInformation(
                    "Using {DbContextType} for database operations.",
                    useMasterNode ? nameof(EmployeeWriteDataContext) : nameof(EmployeeReadDataContext)
                );

                return useMasterNode
                    ? serviceProvider.GetRequiredService<EmployeeWriteDataContext>()
                    : serviceProvider.GetRequiredService<EmployeeReadDataContext>();
            }
        );

        services.AddScoped<DbContext>(
            provider => provider.GetRequiredService<EmployeeDataContext>()
        );
        services.AddSingleton<IDbContextFactory<EmployeeDataContext>, EmployeeDataContextFactory>();
        services.AddDbContextFactory<EmployeeDataContext>();
        services.AddForwardingDbContextFactory<DbContext, EmployeeDataContext>();
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
                    _,
                    options
                ) =>
                {
                    options
                        .UseNpgsql(
                            membershipConnectionString,
                            sqlOptions =>
                            {
                                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                            }
                        )
                        .LogTo(Console.WriteLine, LogLevel.Information)
                        .EnableDetailedErrors(false)
                        .EnableSensitiveDataLogging(false);

                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                    {
                        options.EnableSensitiveDataLogging();
                    }
                }
            );
        }

        services.AddKeyedScoped<DbContext>(
            nameof(MembershipFacilityExternalDbContext),
            (
                provider,
                _
            ) => provider.GetRequiredService<MembershipFacilityExternalDbContext>()
        );
    }
}
