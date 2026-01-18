using Liberty.GmoPaymentGateway.Options;
using Liberty.Reservation.Application.Behaviors;
using Liberty.Reservation.User.Application.Contexts;
using Liberty.Reservation.User.Application.Contexts.Interceptors;
using Liberty.Reservation.User.Application.Domains.Repositories;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.WebAPI.Application.ExternalServices.Membership.Facility;
using Liberty.Reservation.User.WebAPI.Configurations;
using Liberty.Reservation.User.WebAPI.Initializations;
using Liberty.Reservation.User.WebAPI.Middlewares;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest.Utilities;
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

namespace Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest;

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
                options.ConfigId = "001";
                options.UrlPayment = "https://kt01.mul-pay.jp/payment/GetLinkplusUrlPayment.json";
                options.RetUrl = "http://localhost:7087/gmo-payment-result";
                options.CompleteUrl = "http://localhost:7087/gmo-payment-result";
                options.CancelUrl = "http://localhost:7087/gmo-payment-result";
                options.PayMethods = "credit";
            }
        );

        services.AddTransient<ISiteRepository, SiteRepository>();
        services.AddTransient<IAppDateRepository, AppDateRepository>();
        services.AddTransient<ICustomerInfoRepository, CustomerInfoRepository>();
        services.AddTransient<IRoomGroupRepository, RoomGroupRepository>();
        services.AddTransient<IPlanRoomGroupRepository, PlanRoomGroupRepository>();
        services.AddTransient<IFacilityPersonAgeTypeRepository, FacilityPersonAgeTypeRepository>();
        services.AddTransient<IPlanOptionItemRepository, PlanOptionItemRepository>();
        services.AddTransient<IOptionItemAppDateRepository, OptionItemAppDateRepository>();
        services.AddTransient<IReservationRoomGroupAppDatePersonAgeTypeRepository, ReservationRoomGroupAppDatePersonAgeTypeRepository>();
    }

    public override void ConfigureMiddleware(
        IApplicationBuilder app,
        IHostEnvironment environment,
        IConfiguration configuration
    )
    {
        environment.EnvironmentName = Environments.Development;

        app
            .UseApplicationDatabase(environment)
            .UseApplicationSecurity(configuration)
            .UseApplicationProblemDetails(environment);

        app.UseWhen(
            context => context.Request.Path.StartsWithSegments("/api/guests"),
            appBuilder =>
            {
                appBuilder.UseMiddleware<CheckGuestCodeValidMiddleware>();
            }
        );
    }

    protected override void AddDatabase(
        IConfiguration
            configuration,
        IServiceCollection
            services
    )
    {
        AddReservationDatabase(services);
        AddMembershipFacilityDatabase(services);
        services.AddHostedService<InitializationService>();
    }

    private static void AddReservationDatabase(
        IServiceCollection
            services
    )
    {
        if (TestUtil.IsUseTestSqlite())
        {
            var connection = new SqliteConnection
            (
                new SqliteConnectionStringBuilder { DataSource = ":memory:" }.ToString()
            );
            services.AddDbContext<UserDataContext>(
                context
                    => context.UseSqlite(connection)
            );
            services.AddTransient(
                _ =>
                {
                    var compiler = new PostgresCompiler();
                    return new QueryFactory(
                        connection,
                        compiler
                    );
                }
            );
        }
        else
        {
            var connectionString = MockPostgreSqlContainer.ReservationConnectionString;
            var connectionReadDataString = string.Empty;

            services.RemoveAll(typeof(DbContextOptions<UserDataContext>));
            services.RemoveAll(typeof(DbContextOptions<UserWriteDataContext>));
            services.RemoveAll(typeof(DbContextOptions<UserReadDataContext>));

            services.AddDbContextPool<UserWriteDataContext>(
                (
                    serviceProvider,
                    options
                ) =>
                {
                    options.UseNpgsql(
                            connectionString,
                            sqlOptions =>
                            {
                                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                                sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                            }
                        )
                        .UseSnakeCaseNamingConvention()
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors(false)
                        .EnableSensitiveDataLogging(false)
                        .AddInterceptors(
                            new AuditSaveChangesInterceptor(
                                serviceProvider.GetRequiredService<IHttpContextAccessor>()
                            ),
                            new CacheSaveChangesInterceptor(serviceProvider)
                        );
                }
            );

            services.AddDbContextPool<UserReadDataContext>(
                (
                    serviceProvider,
                    options
                ) =>
                {
                    options.UseNpgsql(
                            connectionReadDataString,
                            sqlOptions =>
                            {
                                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                                sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                            }
                        )
                        .UseSnakeCaseNamingConvention()
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors(false)
                        .EnableSensitiveDataLogging(false)
                        .AddInterceptors(
                            new AuditSaveChangesInterceptor(
                                serviceProvider.GetRequiredService<IHttpContextAccessor>()
                            )
                        );
                }
            );

            services.AddScoped<UserDataContext>(
                serviceProvider =>
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<UserDataContext>>();

                    DbContextTypeHolder.UseMasterDb.Value = true;
                    var useMasterNode = DbContextTypeHolder.UseMasterDb.Value;

                    logger.LogInformation(
                        "Using {DbContextType} for database operations.",
                        useMasterNode ? nameof(UserWriteDataContext) : nameof(UserReadDataContext)
                    );

                    return useMasterNode
                        ? serviceProvider.GetRequiredService<UserWriteDataContext>()
                        : serviceProvider.GetRequiredService<UserReadDataContext>();
                }
            );
            services.AddScoped<DbContext>(
                provider => provider.GetRequiredService<UserDataContext>()
            );
            services.AddSingleton<IDbContextFactory<UserDataContext>, UserDataContextFactory>();
            services.AddDbContextFactory<UserDataContext>();
            services.AddForwardingDbContextFactory<DbContext, UserDataContext>();

            services.AddTransient
            (
                _
                    =>
                {
                    var connection = new NpgsqlConnection(connectionString);
                    var compiler = new PostgresCompiler();
                    return new QueryFactory(
                        connection,
                        compiler
                    );
                }
            );
        }

        services.AddScoped<DbContext>(
            provider => provider.GetRequiredService<UserDataContext>()
        );

        services.AddForwardingDbContextFactory<DbContext, UserDataContext>();

        services.AddDbContextFactory<UserDataContext>();
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
            services.AddDbContext<MembershipFacilityExternalDbContext>(
                context => context.UseSqlite(connection)
            );
        }
        else
        {
            var membershipConnectionString = MockPostgreSqlContainer.FacilityConnectionString;
            services.RemoveAll(typeof(DbContextOptions<MembershipFacilityExternalDbContext>));
            services.AddDbContext<MembershipFacilityExternalDbContext>(
                (
                    serviceProvider,
                    options
                ) =>
                {
                    options.UseNpgsql(
                            membershipConnectionString,
                            sqlOptions => sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null)
                        )
                        .UseSnakeCaseNamingConvention()
                        .LogTo(Console.WriteLine, LogLevel.Information)
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors(false)
                        .EnableSensitiveDataLogging(false)
                        .AddInterceptors(new CacheSaveChangesInterceptor(serviceProvider));
                }
            );
        }
    }
}
