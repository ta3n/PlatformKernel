using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Liberty.Cache.Services;
using Liberty.Media.Services;
using Liberty.Reservation.Site.File.WebAPI.Configurations;
using Liberty.Reservation.Site.File.WebAPI.Test.Configuration;
using Liberty.Reservation.Site.File.WebAPI.Test.InfrastructureOfTest.Utilities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using IStartup = Liberty.Reservation.Site.File.WebAPI.Startup.IStartup;

namespace Liberty.Reservation.Site.File.WebAPI.Test.InfrastructureOfTest;

public class AppWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>, IAsyncLifetime
    where TEntryPoint : class, IStartup, new()
{
    private IServiceProvider? _serviceProvider;
    private readonly IStartup _startup = new TEntryPoint();

    private readonly IContainer _postgreSqlContainer = new PostgreSqlBuilder()
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
        return WebHost.CreateDefaultBuilder()
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
            .UseSolutionRelativeContentRoot("src/services/site/Liberty.Reservation.Site.File.WebAPI")
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
                            typeof(IAwsS3Service),
                            typeof(MockAwsS3Service),
                            ServiceLifetime.Scoped
                        )
                    );
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
                            .UseApplicationDatabase(
                                context.HostingEnvironment,
                                true
                            )
                            .UseMembershipDatabase(
                                context.HostingEnvironment,
                                true
                            ),
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

    public TService? GetRequiredService<TService>() where TService : class
    {
        return _serviceProvider?.GetRequiredService<TService>();
    }

    public async Task InitializeAsync()
    {
        if (!TestUtil.IsUseTestSqlite())
        {
            await _postgreSqlContainer.StartAsync();

            await _postgreSqlContainer.ExecAsync(
                [
                    "psql",
                    "-U",
                    "postgres",
                    "-c",
                    $"CREATE DATABASE {MockPostgreSqlContainer.FacilityDatabase};"
                ]
            );

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
}
