using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.ElasticSearch.Abstractions;
using SharedKernel.ElasticSearch.HealthChecks;
using SharedKernel.ElasticSearch.Options;
using SharedKernel.ElasticSearch.Services;

namespace SharedKernel.ElasticSearch;

/// <summary>
/// Provides extension methods for registering the ElasticSearch plugin.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Registers configuration, client, services, and health checks for ElasticSearch.
    /// </summary>
    public static IServiceCollection AddElasticSearch(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = ElasticSearchOptions.SectionName
    )
    {
        var section = configuration.GetSection(sectionName);
        var elasticSearchOptions = section.Get<ElasticSearchOptions>() ?? new ElasticSearchOptions();

        services.Configure<ElasticSearchOptions>(section);
        services.AddSingleton(
            static sp => sp.GetRequiredService<IOptions<ElasticSearchOptions>>().Value
        );

        if (!elasticSearchOptions.Enabled)
        {
            return services;
        }

        ValidateOptions(elasticSearchOptions);

        services.AddSingleton(
            static sp =>
            {
                var options = sp.GetRequiredService<ElasticSearchOptions>();
                var logger = sp.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("SharedKernel.ElasticSearch.Transport");

                var settings = CreateClientSettings(options, logger);
                return new ElasticsearchClient(settings);
            }
        );
        services.AddSingleton<IElasticSearchClientFactory, ElasticSearchClientFactory>();
        services.AddSingleton<IElasticSearchIndexNameResolver>(
            static sp => new ElasticSearchIndexNameResolver(sp.GetRequiredService<ElasticSearchOptions>())
        );
        services.AddSingleton<IElasticSearchService>(
            static sp => new ElasticSearchService(
                sp.GetRequiredService<ElasticsearchClient>(),
                sp.GetRequiredService<IElasticSearchIndexNameResolver>(),
                sp.GetRequiredService<ElasticSearchOptions>()
            )
        );

        if (elasticSearchOptions.HealthCheck.Enabled)
        {
            services.AddHealthChecks().AddCheck<ElasticSearchHealthCheck>(
                elasticSearchOptions.HealthCheck.Name,
                elasticSearchOptions.HealthCheck.FailureStatus,
                elasticSearchOptions.HealthCheck.Tags
            );
        }

        return services;
    }

    private static ElasticsearchClientSettings CreateClientSettings(
        ElasticSearchOptions options,
        ILogger logger
    )
    {
        var settings = CreateBaseSettings(options);

        if (!string.IsNullOrWhiteSpace(options.DefaultIndex))
        {
            settings = settings.DefaultIndex(options.DefaultIndex);
        }

        settings = settings
            .EnableHttpCompression(options.EnableHttpCompression)
            .ThrowExceptions(options.ThrowExceptions)
            .DisablePing(options.DisablePing)
            .ConnectionLimit(options.ConnectionLimit)
            .MaximumRetries(options.MaximumRetries)
            .RequestTimeout(TimeSpan.FromSeconds(options.RequestTimeoutSeconds))
            .PingTimeout(TimeSpan.FromSeconds(options.PingTimeoutSeconds))
            .DeadTimeout(TimeSpan.FromSeconds(options.DeadTimeoutSeconds))
            .MaxDeadTimeout(TimeSpan.FromSeconds(options.MaxDeadTimeoutSeconds))
            .MaxRetryTimeout(TimeSpan.FromSeconds(options.MaxRetryTimeoutSeconds))
            .SniffOnStartup(options.SniffOnStartup)
            .SniffOnConnectionFault(options.SniffOnConnectionFault)
            .EnableTcpKeepAlive(
                TimeSpan.FromSeconds(options.TcpKeepAliveTimeSeconds),
                TimeSpan.FromSeconds(options.TcpKeepAliveIntervalSeconds)
            )
            .OnRequestCompleted(
                apiCallDetails =>
                {
                    if (!logger.IsEnabled(LogLevel.Debug))
                    {
                        return;
                    }

                    logger.LogDebug(
                        "ElasticSearch request completed. Method: {Method}. Uri: {Uri}. StatusCode: {StatusCode}.",
                        apiCallDetails.HttpMethod,
                        apiCallDetails.Uri,
                        apiCallDetails.HttpStatusCode
                    );
                }
            );

        if (!string.IsNullOrWhiteSpace(options.CertificateFingerprint))
        {
            settings = settings.CertificateFingerprint(options.CertificateFingerprint);
        }

        if (options.AllowUnsafeServerCertificate)
        {
            settings = settings.ServerCertificateValidationCallback(CertificateValidations.AllowAll);
        }

        if (options.EnableDebugMode)
        {
            settings = settings.EnableDebugMode();
        }

        if (options.PrettyJson)
        {
            settings = settings.PrettyJson();
        }

        if (options.DisableDirectStreaming)
        {
            settings = settings.DisableDirectStreaming();
        }

        var authorizationHeader = CreateAuthorizationHeader(options);
        if (authorizationHeader is not null && string.IsNullOrWhiteSpace(options.CloudId))
        {
            settings = settings.Authentication(authorizationHeader);
        }

        return settings;
    }

    private static ElasticsearchClientSettings CreateBaseSettings(
        ElasticSearchOptions options
    )
    {
        var authorizationHeader = CreateAuthorizationHeader(options);

        if (!string.IsNullOrWhiteSpace(options.CloudId))
        {
            if (authorizationHeader is null)
            {
                throw new InvalidOperationException(
                    "ElasticSearch:CloudId requires ElasticSearch:ApiKey or ElasticSearch:Username/Password."
                );
            }

            return new ElasticsearchClientSettings(options.CloudId, authorizationHeader);
        }

        return new ElasticsearchClientSettings(
            new Uri(options.Endpoint!, UriKind.Absolute)
        );
    }

    private static AuthorizationHeader? CreateAuthorizationHeader(
        ElasticSearchOptions options
    )
    {
        if (!string.IsNullOrWhiteSpace(options.ApiKey))
        {
            return new ApiKey(options.ApiKey);
        }

        if (
            !string.IsNullOrWhiteSpace(options.Username)
            && !string.IsNullOrWhiteSpace(options.Password)
        )
        {
            return new BasicAuthentication(options.Username, options.Password);
        }

        return null;
    }

    private static void ValidateOptions(
        ElasticSearchOptions options
    )
    {
        if (string.IsNullOrWhiteSpace(options.Endpoint) && string.IsNullOrWhiteSpace(options.CloudId))
        {
            throw new InvalidOperationException(
                "ElasticSearch configuration requires either ElasticSearch:Endpoint or ElasticSearch:CloudId."
            );
        }

        if (!string.IsNullOrWhiteSpace(options.Endpoint) && !Uri.TryCreate(options.Endpoint, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException(
                "ElasticSearch:Endpoint must be a valid absolute URI."
            );
        }

        var hasBasicAuthentication = !string.IsNullOrWhiteSpace(options.Username)
            || !string.IsNullOrWhiteSpace(options.Password);
        var hasApiKeyAuthentication = !string.IsNullOrWhiteSpace(options.ApiKey);

        if (hasBasicAuthentication && hasApiKeyAuthentication)
        {
            throw new InvalidOperationException(
                "Use either ElasticSearch:ApiKey or ElasticSearch:Username/Password, not both."
            );
        }

        if (options.AllowUnsafeServerCertificate && !string.IsNullOrWhiteSpace(options.CertificateFingerprint))
        {
            throw new InvalidOperationException(
                "Use either ElasticSearch:CertificateFingerprint or ElasticSearch:AllowUnsafeServerCertificate, not both."
            );
        }

        if (hasBasicAuthentication && (string.IsNullOrWhiteSpace(options.Username) || string.IsNullOrWhiteSpace(options.Password)))
        {
            throw new InvalidOperationException(
                "ElasticSearch basic authentication requires both Username and Password."
            );
        }

        if (options.MaximumRetries < 0)
        {
            throw new InvalidOperationException("ElasticSearch:MaximumRetries must be greater than or equal to zero.");
        }

        if (
            options.ConnectionLimit <= 0
            || options.RequestTimeoutSeconds <= 0
            || options.PingTimeoutSeconds <= 0
            || options.DeadTimeoutSeconds <= 0
            || options.MaxDeadTimeoutSeconds <= 0
            || options.MaxRetryTimeoutSeconds <= 0
            || options.TcpKeepAliveTimeSeconds <= 0
            || options.TcpKeepAliveIntervalSeconds <= 0
        )
        {
            throw new InvalidOperationException(
                "ElasticSearch timeout and connection settings must be greater than zero."
            );
        }
    }
}
