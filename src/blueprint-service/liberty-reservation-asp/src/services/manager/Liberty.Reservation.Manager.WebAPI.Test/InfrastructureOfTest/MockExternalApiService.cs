using Liberty.Reservation.Manager.WebAPI.Application.Web.ApiService;
using Microsoft.AspNetCore.Http;

namespace Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;

public class MockExternalApiService : IExternalApiService
{
    public Task<(HeaderDictionary header, string content)> DeleteAsync(
        ExternalService externalService,
        string queryRequest,
        CancellationToken cancellationToken = default
    )
    {
        var result = (new HeaderDictionary(), string.Empty);
        return Task.FromResult(result);
    }

    public Task<(HeaderDictionary header, string content)> GetAsync(
        ExternalService externalService,
        string queryRequest,
        CancellationToken cancellationToken = default
    )
    {
        var result = (new HeaderDictionary(), string.Empty);
        return Task.FromResult(result);
    }

    public Task<(HeaderDictionary header, string content)> GetAsync(
        ExternalService externalService,
        string queryRequest,
        Dictionary<string, string>? customHeaders,
        CancellationToken cancellationToken = default
    )
    {
        var result = (new HeaderDictionary(), string.Empty);
        return Task.FromResult(result);
    }

    public Task<(HeaderDictionary header, string content)> PatchAsync(
        ExternalService externalService,
        string queryRequest,
        string content,
        CancellationToken cancellationToken = default
    )
    {
        var result = (new HeaderDictionary(), string.Empty);
        return Task.FromResult(result);
    }

    public Task<(HeaderDictionary header, string content)> PostAsync(
        ExternalService externalService,
        string queryRequest,
        string content,
        CancellationToken cancellationToken = default
    )
    {
        var result = (new HeaderDictionary(), string.Empty);
        return Task.FromResult(result);
    }

    public Task<(HeaderDictionary header, string content)> PutAsync(
        ExternalService externalService,
        string queryRequest,
        string content,
        CancellationToken cancellationToken = default
    )
    {
        var result = (new HeaderDictionary(), string.Empty);
        return Task.FromResult(result);
    }
}
