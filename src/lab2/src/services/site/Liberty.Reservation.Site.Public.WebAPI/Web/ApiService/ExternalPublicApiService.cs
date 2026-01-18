using System.Text;
using HttpClientToCurl;
using Liberty.Reservation.Site.Public.WebAPI.Models;
using Liberty.Reservation.Site.Public.WebAPI.Settings;
using Microsoft.Extensions.Options;

namespace Liberty.Reservation.Site.Public.WebAPI.Web.ApiService;

public class ExternalPublicApiService(
    IHttpClientFactory httpClientFactory,
    IOptions<ServiceSetting> serviceSettingOption,
    IHostEnvironment hostEnvironment,
    IHttpContextAccessor httpContextAccessor
) : IExternalPublicApiService
{
    private ServiceSetting ServiceSetting { get; } =
        serviceSettingOption.Value ?? throw new ArgumentNullException(nameof(serviceSettingOption));

    private static readonly List<string> SkipHeaders =
    [
        "Origin",
        "Referer",
        "Host",
        "Content-Length",
        "Connection",
        "Accept-Encoding",
        "Transfer-Encoding",
        "TE",
        "Trailer",
        "Upgrade",
        "Keep-Alive",
        "Proxy-Authorization",
        "Proxy-Authenticate",
        "Proxy-Connection"
    ];

    private static readonly List<string> AllowHeaders =
    [
        "Authorization",
        "Accept-Language",
        "Time-Zone-Offset",
        "X-Facility-Code",
        "X-Site-Code"
    ];

    private static HeaderDictionary GetHeadersFromHttpResponseHeader(
        HttpResponseMessage response
    )
    {
        var headers = new HeaderDictionary();

        foreach (var httpResponseHeader in response.Headers)
        {
            if (httpResponseHeader.Key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            headers[httpResponseHeader.Key] = string.Join(", ", httpResponseHeader.Value);
        }

        foreach (var httpResponseHeader in response.Content.Headers)
        {
            headers[httpResponseHeader.Key] = string.Join(", ", httpResponseHeader.Value);
        }

        return headers;
    }

    private ServiceConfig? GetServiceConfig(
        ExternalService externalService
    )
    {
        return externalService switch
        {
            ExternalService.SiteService => ServiceSetting.SiteService,
            _ => throw new ArgumentOutOfRangeException(nameof(externalService), externalService, null)
        };
    }

    private async Task<HttpResponseModel> SendAsync(
        ExternalService externalService,
        Func<ServiceConfig?, HttpRequestMessage> requestFactory,
        CancellationToken cancellationToken = default
    )
    {
        using var httpClient = httpClientFactory.CreateClient();

        var serviceConfig = GetServiceConfig(externalService);

        var request = requestFactory.Invoke(serviceConfig);

        var context = httpContextAccessor.HttpContext;

        if (context?.Request.Headers is { Count: > 0 } headers)
        {
            foreach (var header in headers)
            {
                if (SkipHeaders.Contains(header.Key, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!AllowHeaders.Contains(header.Key, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                request.Headers.TryAddWithoutValidation(header.Key, [.. header.Value]);
            }
        }

        if (hostEnvironment.IsDevelopment())
        {
            httpClient.GenerateCurlInConsole(
                request,
                config =>
                {
                    config.TurnOn = true;
                    config.NeedAddDefaultHeaders = true;
                    config.EnableCodeBeautification = false;
                }
            );
        }

        using var response = await httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken
        );

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var headerResponse = GetHeadersFromHttpResponseHeader(response);

        return new HttpResponseModel
        {
            Headers = headerResponse,
            Content = bytes,
            StatusCode = response.StatusCode
        };
    }

    public async Task<HttpResponseModel> GetAsync(
        ExternalService externalService,
        string queryRequest,
        CancellationToken cancellationToken = default
    )
    {
        return await SendAsync(
            externalService,
            config =>
            {
                var requestUri = $"{config?.Url}/{queryRequest}";

                var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    requestUri
                );

                return request;
            },
            cancellationToken
        );
    }

    public async Task<HttpResponseModel> PostAsync(
        ExternalService externalService,
        string queryRequest,
        string content,
        CancellationToken cancellationToken = default
    )
    {
        return await SendAsync(
            externalService,
            config =>
            {
                var requestUri = $"{config?.Url}/{queryRequest}";

                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    requestUri
                )
                {
                    Content = new StringContent(
                        content,
                        Encoding.UTF8,
                        "application/json"
                    )
                };

                return request;
            },
            cancellationToken
        );
    }

    public async Task<HttpResponseModel> PutAsync(
        ExternalService externalService,
        string queryRequest,
        string content,
        CancellationToken cancellationToken = default
    )
    {
        return await SendAsync(
            externalService,
            config =>
            {
                var requestUri = $"{config?.Url}/{queryRequest}";

                var request = new HttpRequestMessage(
                    HttpMethod.Put,
                    requestUri
                )
                {
                    Content = new StringContent(
                        content,
                        Encoding.UTF8,
                        "application/json"
                    )
                };

                return request;
            },
            cancellationToken
        );
    }

    public async Task<HttpResponseModel> PatchAsync(
        ExternalService externalService,
        string queryRequest,
        string content,
        CancellationToken cancellationToken = default
    )
    {
        return await SendAsync(
            externalService,
            config =>
            {
                var requestUri = $"{config?.Url}/{queryRequest}";

                var request = new HttpRequestMessage(
                    HttpMethod.Patch,
                    requestUri
                )
                {
                    Content = new StringContent(
                        content,
                        Encoding.UTF8,
                        "application/json"
                    )
                };

                return request;
            },
            cancellationToken
        );
    }

    public async Task<HttpResponseModel> DeleteAsync(
        ExternalService externalService,
        string queryRequest,
        CancellationToken cancellationToken = default
    )
    {
        return await SendAsync(
            externalService,
            config =>
            {
                var requestUri = $"{config?.Url}/{queryRequest}";

                var request = new HttpRequestMessage(
                    HttpMethod.Delete,
                    requestUri
                );

                return request;
            },
            cancellationToken
        );
    }
}
