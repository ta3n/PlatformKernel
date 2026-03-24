using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.ElasticSearch.Abstractions;
using Testcontainers.Elasticsearch;

namespace SharedKernel.ElasticSearch.Test.Integration;

public sealed class ElasticSearchContainerFixture : IAsyncLifetime
{
    public const string Username = "elastic";
    public const string Password = "shared-kernel-test-password";

    private readonly ElasticsearchContainer _elasticSearchContainer = new ElasticsearchBuilder()
        .WithPassword(Password)
        .WithEnvironment("ES_JAVA_OPTS", "-Xms512m -Xmx512m")
        .Build();

    public string ConnectionString => _elasticSearchContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _elasticSearchContainer.StartAsync();
        await WaitForClusterReadyAsync();
    }

    public async Task DisposeAsync()
    {
        await _elasticSearchContainer.DisposeAsync();
    }

    public ServiceProvider CreateServiceProvider(
        string? defaultIndex = null
    )
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("ElasticSearch:Enabled", "true"),
                new KeyValuePair<string, string?>("ElasticSearch:Endpoint", ConnectionString),
                new KeyValuePair<string, string?>("ElasticSearch:Username", Username),
                new KeyValuePair<string, string?>("ElasticSearch:Password", Password),
                new KeyValuePair<string, string?>("ElasticSearch:AllowUnsafeServerCertificate", "true"),
                new KeyValuePair<string, string?>("ElasticSearch:DefaultIndex", defaultIndex),
                new KeyValuePair<string, string?>("ElasticSearch:DisablePing", "true"),
                new KeyValuePair<string, string?>("ElasticSearch:ThrowExceptions", "true"),
                new KeyValuePair<string, string?>("ElasticSearch:MaximumRetries", "0"),
                new KeyValuePair<string, string?>("ElasticSearch:RequestTimeoutSeconds", "10"),
                new KeyValuePair<string, string?>("ElasticSearch:PingTimeoutSeconds", "3"),
                new KeyValuePair<string, string?>("ElasticSearch:DeadTimeoutSeconds", "5"),
                new KeyValuePair<string, string?>("ElasticSearch:MaxDeadTimeoutSeconds", "15"),
                new KeyValuePair<string, string?>("ElasticSearch:MaxRetryTimeoutSeconds", "10"),
                new KeyValuePair<string, string?>("ElasticSearch:HealthCheck:Enabled", "true"),
                new KeyValuePair<string, string?>("ElasticSearch:HealthCheck:Name", "elasticsearch"),
                new KeyValuePair<string, string?>("ElasticSearch:HealthCheck:Tags:0", "ready")
            ])
            .Build();

        services.AddLogging();
        services.AddOptions();
        services.AddElasticSearch(configuration);

        return services.BuildServiceProvider(validateScopes: true);
    }

    public async Task DeleteIndexIfExistsAsync(
        string index
    )
    {
        using var provider = CreateServiceProvider();
        var service = provider.GetRequiredService<IElasticSearchService>();
        var response = await service.Client.Indices.ExistsAsync(index);

        if (response.Exists)
        {
            await service.Client.Indices.DeleteAsync(index);
        }
    }

    private async Task WaitForClusterReadyAsync()
    {
        using var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri($"{ConnectionString.TrimEnd('/')}/"),
            Timeout = TimeSpan.FromSeconds(5)
        };

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            credentials
        );

        Exception? lastException = null;
        var deadline = DateTimeOffset.UtcNow.AddSeconds(90);

        while (DateTimeOffset.UtcNow < deadline)
        {
            try
            {
                using var response = await httpClient.GetAsync(
                    "_cluster/health?wait_for_status=yellow&timeout=5s"
                );

                if (response.IsSuccessStatusCode)
                {
                    return;
                }

                lastException = new InvalidOperationException(
                    $"ElasticSearch cluster health returned {(int)response.StatusCode} {response.ReasonPhrase}."
                );
            }
            catch (Exception exception)
            {
                lastException = exception;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        throw new TimeoutException(
            "ElasticSearch container started but the cluster did not become ready in time.",
            lastException
        );
    }
}
