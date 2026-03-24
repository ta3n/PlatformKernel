using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SharedKernel.ElasticSearch.Abstractions;
using SharedKernel.ElasticSearch.Attributes;
using SharedKernel.ElasticSearch.Options;

namespace SharedKernel.ElasticSearch.Test;

public sealed class ElasticSearchExtensionsTests
{
    [Fact]
    public void AddElasticSearch_DoesNotRegisterClient_WhenDisabled()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(
        [
            new KeyValuePair<string, string?>("ElasticSearch:Enabled", "false")
        ]);

        services.AddLogging();
        services.AddOptions();

        services.AddElasticSearch(configuration);

        using var provider = services.BuildServiceProvider();

        Assert.Null(provider.GetService<IElasticSearchService>());
        Assert.Null(provider.GetService<IElasticSearchClientFactory>());
    }

    [Fact]
    public void AddElasticSearch_RegistersClientAndHealthCheck_WhenEnabled()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(
        [
            new KeyValuePair<string, string?>("ElasticSearch:Enabled", "true"),
            new KeyValuePair<string, string?>("ElasticSearch:Endpoint", "http://localhost:9200"),
            new KeyValuePair<string, string?>("ElasticSearch:DefaultIndex", "default-index"),
            new KeyValuePair<string, string?>("ElasticSearch:HealthCheck:Enabled", "true"),
            new KeyValuePair<string, string?>("ElasticSearch:HealthCheck:Name", "elastic-ready"),
            new KeyValuePair<string, string?>("ElasticSearch:HealthCheck:Tags:0", "ready")
        ]);

        services.AddLogging();
        services.AddOptions();

        services.AddElasticSearch(configuration);

        using var provider = services.BuildServiceProvider();

        var service = provider.GetRequiredService<IElasticSearchService>();
        var healthCheckOptions = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<HealthCheckServiceOptions>>();

        Assert.NotNull(service.Client);
        Assert.Equal("default-index", service.ResolveIndex<UndecoratedDocument>());
        Assert.Contains(healthCheckOptions.Value.Registrations, registration => registration.Name == "elastic-ready");
    }

    [Fact]
    public void AddElasticSearch_Throws_WhenEnabledWithoutEndpointOrCloudId()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(
        [
            new KeyValuePair<string, string?>("ElasticSearch:Enabled", "true")
        ]);

        services.AddLogging();
        services.AddOptions();

        var exception = Assert.Throws<InvalidOperationException>(
            () => services.AddElasticSearch(configuration)
        );

        Assert.Contains("Endpoint", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AddElasticSearch_Throws_WhenUnsafeCertificateAndFingerprintAreBothConfigured()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(
        [
            new KeyValuePair<string, string?>("ElasticSearch:Enabled", "true"),
            new KeyValuePair<string, string?>("ElasticSearch:Endpoint", "https://localhost:9200"),
            new KeyValuePair<string, string?>("ElasticSearch:AllowUnsafeServerCertificate", "true"),
            new KeyValuePair<string, string?>("ElasticSearch:CertificateFingerprint", "abc")
        ]);

        services.AddLogging();
        services.AddOptions();

        var exception = Assert.Throws<InvalidOperationException>(
            () => services.AddElasticSearch(configuration)
        );

        Assert.Contains("CertificateFingerprint", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Resolver_UsesAttribute_BeforeDefaultIndex()
    {
        var resolver = new Services.ElasticSearchIndexNameResolver(
            new ElasticSearchOptions
            {
                DefaultIndex = "fallback-index"
            }
        );

        var index = resolver.Resolve<AttributedDocument>();

        Assert.Equal("orders-v1", index);
    }

    [Fact]
    public void Resolver_UsesConfiguredTypeMapping_WhenAvailable()
    {
        var resolver = new Services.ElasticSearchIndexNameResolver(
            new ElasticSearchOptions
            {
                DefaultIndex = "fallback-index",
                Indexes = new Dictionary<string, string>
                {
                    [nameof(UndecoratedDocument)] = "mapped-index"
                }
            }
        );

        var index = resolver.Resolve<UndecoratedDocument>();

        Assert.Equal("mapped-index", index);
    }

    private static IConfiguration CreateConfiguration(
        IEnumerable<KeyValuePair<string, string?>> values
    )
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }

    [ElasticSearchIndex("orders-v1")]
    private sealed class AttributedDocument
    {
        public string Id { get; init; } = string.Empty;
    }

    private sealed class UndecoratedDocument
    {
        public string Id { get; init; } = string.Empty;
    }
}
