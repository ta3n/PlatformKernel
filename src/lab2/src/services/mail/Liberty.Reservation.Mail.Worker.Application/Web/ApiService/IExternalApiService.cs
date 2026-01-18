using Microsoft.AspNetCore.Http;

namespace Liberty.Reservation.Mail.Worker.Application.Web.ApiService;

public interface IExternalApiService
{
    Task<(HeaderDictionary header, string content)> GetAsync(
        ExternalService externalService,
        string queryRequest,
        CancellationToken cancellationToken = default
    );

    Task<(HeaderDictionary header, string content)> PostAsync(
        ExternalService externalService,
        string queryRequest,
        string content,
        CancellationToken cancellationToken = default
    );

    Task<(HeaderDictionary header, string content)> PutAsync(
        ExternalService externalService,
        string queryRequest,
        string content,
        CancellationToken cancellationToken = default
    );

    Task<(HeaderDictionary header, string content)> PatchAsync(
        ExternalService externalService,
        string queryRequest,
        string content,
        CancellationToken cancellationToken = default
    );

    Task<(HeaderDictionary header, string content)> DeleteAsync(
        ExternalService externalService,
        string queryRequest,
        CancellationToken cancellationToken = default
    );
}
