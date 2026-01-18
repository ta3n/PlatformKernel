using System.Security.Claims;
using DotNet.Testcontainers.Builders;
using Liberty.ApplicationShared.Domains.Services.Mails;
using Liberty.Cache.Services;
using Liberty.Reservation.Employee.WebAPI.Configurations;
using Liberty.Reservation.Employee.WebAPI.Test.Configuration;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Testcontainers.PostgreSql;
using IStartup = Liberty.Reservation.Employee.WebAPI.Startup.IStartup;

namespace Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

public class AppWebApplicationFactory<TEntryPoint>
    : WebApplicationFactory<TEntryPoint>,
        IAsyncLifetime
    where TEntryPoint : class, IStartup, new()
{
    private IServiceProvider? _serviceProvider;
    private readonly IStartup _startup = new TEntryPoint();
    private ClaimsPrincipal? Identity { get; set; }

    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16.4")
        .WithPortBinding(MockPostgreSqlContainer.ContainerPort, true)
        .WithDatabase(MockPostgreSqlContainer.ReservationDatabase)
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithAutoRemove(true)
        .WithCleanUp(true)
        .WithWaitStrategy(
            Wait.ForUnixContainer()
                .UntilPortIsAvailable(MockPostgreSqlContainer.ContainerPort)
                .UntilCommandIsCompleted(
                    $"pg_isready -h {MockPostgreSqlContainer.Host} -p {MockPostgreSqlContainer.ContainerPort}"
                )
        )
        .Build();

    protected override IWebHostBuilder CreateWebHostBuilder()
    {
        return WebHost
            .CreateDefaultBuilder()
            .ConfigureServices(
                (
                    context,
                    services
                ) =>
                {
                    _startup.Configure(context.Configuration, services);
                    _startup.ConfigureServices(
                        services,
                        context.Configuration,
                        context.HostingEnvironment
                    );
                }
            );
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder
    )
    {
        builder
            .UseSolutionRelativeContentRoot(
                "src/services/employee/Liberty.Reservation.Employee.WebAPI"
            )
            .ConfigureServices(
                services =>
                {
                    services.AddMvc(TestMvcStartup.ConfigureMvcAuthorization());
                    services.Replace(
                        new ServiceDescriptor(
                            typeof(IHttpContextFactory),
                            typeof(MockHttpContextFactory),
                            ServiceLifetime.Scoped
                        )
                    );
                    services.Replace(
                        new ServiceDescriptor(
                            typeof(ICacheService),
                            typeof(MockCacheService),
                            ServiceLifetime.Scoped
                        )
                    );
                    services.Replace(
                        new ServiceDescriptor(
                            typeof(IMailService),
                            typeof(MockMailService),
                            ServiceLifetime.Scoped
                        )
                    );
                    services.AddTransient(_ => new MockClaimsPrincipalProvider(Identity!));
                }
            )
            .Configure(
                (
                    context,
                    applicationBuilder
                ) =>
                {
                    _serviceProvider = applicationBuilder.ApplicationServices;
                    _startup.ConfigureMiddleware(
                        applicationBuilder
                            .UseApplicationDatabase(context.HostingEnvironment, true)
                            .UseMembershipFacilityDatabase(context.HostingEnvironment, true),
                        context.HostingEnvironment,
                        context.Configuration
                    );
                    _startup.ConfigureEndpoints(
                        applicationBuilder,
                        context.HostingEnvironment,
                        context.Configuration
                    );
                }
            );
    }

    public TService? GetRequiredService<TService>()
        where TService : class
    {
        return _serviceProvider?.GetRequiredService<TService>();
    }

    public AppWebApplicationFactory<TEntryPoint> WithMockIdentity(
        string name = "user",
        string authenticationType = "MockAuthenticationType"
    )
    {
        Identity = BuildClaimsPrincipal(name, authenticationType);
        return this;
    }

    private static ClaimsPrincipal BuildClaimsPrincipal(
        string name,
        string authenticationType
    )
    {
        var claims = new List<Claim>
        {
            new("Sub", name),
            new("FacilityCode", "01j5agp1f4d6s0nsqamp27wp73")
        };
        return new ClaimsPrincipal(new ClaimsIdentity([.. claims], authenticationType));
    }

    public async Task InitializeAsync()
    {
        if (!TestUtil.IsUseTestSqlite())
        {
            await _postgreSqlContainer.StartAsync();

            await EnsureDatabaseCreatedAsync();

            MockPostgreSqlContainer.HostPort = _postgreSqlContainer.GetMappedPublicPort(
                MockPostgreSqlContainer.ContainerPort
            );
        }
    }

    public new async Task DisposeAsync()
    {
        if (!TestUtil.IsUseTestSqlite())
        {
            await _postgreSqlContainer.DisposeAsync();
        }
    }

    private async Task EnsureDatabaseCreatedAsync()
    {
        var originalConnectionString = _postgreSqlContainer.GetConnectionString();
        var builder = new NpgsqlConnectionStringBuilder(originalConnectionString)
        {
            Database = "postgres",
            SslMode = SslMode.Disable,
            Pooling = false
        };
        var connectionString = builder.ToString();
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var checkDbCommand = connection.CreateCommand();
        checkDbCommand.CommandText =
            $"SELECT 1 FROM pg_database WHERE datname = '{MockPostgreSqlContainer.FacilityDatabase}';";

        var exists = await checkDbCommand.ExecuteScalarAsync();
        if (exists is null)
        {
            var createDbCommand = connection.CreateCommand();
            createDbCommand.CommandText =
                $"CREATE DATABASE \"{MockPostgreSqlContainer.FacilityDatabase}\";";
            await createDbCommand.ExecuteNonQueryAsync();
        }
    }
}
