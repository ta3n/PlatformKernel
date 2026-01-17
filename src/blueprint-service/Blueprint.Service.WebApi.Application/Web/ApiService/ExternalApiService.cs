using Microsoft.Extensions.Options;
using System.Text;
using Blueprint.Service.WebApi.Application.Settings;

namespace Blueprint.Service.WebApi.Application.Web.ApiService;

public class ExternalApiService(
    IHttpClientFactory httpClientFactory,
    IOptions<ServiceSetting> serviceSettingOption
) : IExternalApiService
{
    private ServiceSetting ServiceSetting { get; } =
        serviceSettingOption.Value ?? throw new ArgumentNullException(nameof(serviceSettingOption));

    private static HeaderDictionary GetHeadersFromHttpResponseHeader(
        HttpResponseMessage response
    )
    {
        var headers = new HeaderDictionary();

        foreach (var httpResponseHeader in response.Headers)
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
            ExternalService.ReservationEmployeeService => ServiceSetting.ReservationEmployeeService,
            ExternalService.BatchSchedulerService => ServiceSetting.BatchSchedulerService,
            ExternalService.MembershipService => ServiceSetting.MembershipFacilityService,
            _ => throw new ArgumentOutOfRangeException(nameof(externalService), externalService, null)
        };
    }

    private static HttpRequestMessage AddAuthenticate(
        HttpRequestMessage request,
        ServiceConfig? serviceConfig
    )
    {
        request.Headers.Add("AccessCode", serviceConfig?.AccessCode);

        return request;
    }

    private async Task<(HeaderDictionary headers, string content)> SendAsync(
        ExternalService externalService,
        Func<ServiceConfig?, HttpRequestMessage> requestFactory,
        CancellationToken cancellationToken = default
    )
    {
        using var httpClient = httpClientFactory.CreateClient();

        var serviceConfig = GetServiceConfig(externalService);

        var request = requestFactory.Invoke(serviceConfig);
        var requestWithAuth = AddAuthenticate(request, serviceConfig);

        var response = await httpClient.SendAsync(
            requestWithAuth,
            cancellationToken
        );

        var data = await response.Content.ReadAsStringAsync(cancellationToken);
        var headers = GetHeadersFromHttpResponseHeader(response);

        return (headers, data);
    }

    public async Task<(HeaderDictionary header, string content)> GetAsync(
        ExternalService externalService,
        string queryRequest,
        Dictionary<string, string>? customHeaders,
        CancellationToken cancellationToken = default
    )
    {
        return await SendAsync(
            externalService,
            config =>
            {
                var requestUri = $"{config?.Url}/{queryRequest}";
                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

                if (customHeaders == null)
                {
                    return request;
                }

                foreach (var header in customHeaders)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                return request;
            },
            cancellationToken
        );
    }

    public async Task<(HeaderDictionary header, string content)> GetAsync(
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

                return new HttpRequestMessage(
                    HttpMethod.Get,
                    requestUri
                );
            },
            cancellationToken
        );
    }

    public async Task<(HeaderDictionary header, string content)> PostAsync(
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
                );
                request.Content = new StringContent(
                    content,
                    Encoding.UTF8,
                    "application/json"
                );

                return request;
            },
            cancellationToken
        );
    }

    public async Task<(HeaderDictionary header, string content)> PutAsync(
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
                );
                request.Content = new StringContent(
                    content,
                    Encoding.UTF8,
                    "application/json"
                );

                return request;
            },
            cancellationToken
        );
    }

    public async Task<(HeaderDictionary header, string content)> PatchAsync(
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
                );
                request.Content = new StringContent(
                    content,
                    Encoding.UTF8,
                    "application/json"
                );

                return request;
            },
            cancellationToken
        );
    }

    public async Task<(HeaderDictionary header, string content)> DeleteAsync(
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
