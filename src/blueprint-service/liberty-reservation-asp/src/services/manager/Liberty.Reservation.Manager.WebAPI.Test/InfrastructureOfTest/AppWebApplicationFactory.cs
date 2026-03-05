using System.Security.Claims;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Services;
using Liberty.Reservation.Manager.WebAPI.Application.Web.ApiService;
using Liberty.Reservation.Manager.WebAPI.Configurations;
using Liberty.Reservation.Manager.WebAPI.Test.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using IStartup = Liberty.Reservation.Manager.WebAPI.Startup.IStartup;

namespace Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;

public class AppWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>, IAsyncLifetime
    where TEntryPoint : class, IStartup, new()
{
    private IServiceProvider? _serviceProvider;
    private readonly IStartup _startup = new TEntryPoint();
    private ClaimsPrincipal? Identity { get; set; }
    private static string ContainerId { get; set; } = string.Empty;

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
            .UseSolutionRelativeContentRoot("src/services/manager/Liberty.Reservation.Manager.WebAPI")
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
                            typeof(IExternalApiService),
                            typeof(MockExternalApiService),
                            ServiceLifetime.Scoped
                        )
                    );

                    services.Replace(
                        new ServiceDescriptor(
                            typeof(ICheckFacilityService),
                            typeof(MockCheckFacilityService),
                            ServiceLifetime.Scoped
                        )
                    );

                    services.Replace(
                        new ServiceDescriptor(
                            typeof(IBookingHoldManagementService),
                            typeof(MockBookingHoldManagementService),
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
                        applicationBuilder.UseApplicationDatabase(
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

    public string GetContainerName()
    {
        return ContainerId;
    }

    public AppWebApplicationFactory<TEntryPoint> WithMockIdentity(
        string facilityCode,
        string name = "user",
        string authenticationType = "MockAuthenticationType"
    )
    {
        Identity = BuildClaimsPrincipal(
            facilityCode,
            name,
            authenticationType
        );
        return this;
    }

    private static ClaimsPrincipal BuildClaimsPrincipal(
        string facilityCode,
        string name,
        string authenticationType
    )
    {
        var claims = new List<Claim>
        {
            new("Sub", name),
            new("FacilityCode", facilityCode)
        };
        return new ClaimsPrincipal(
            new ClaimsIdentity([.. claims], authenticationType)
        );
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        ContainerId = _postgreSqlContainer.Id;

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

    public new async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
    }
}
