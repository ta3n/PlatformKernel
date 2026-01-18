using Liberty.Reservation.Site.Public.WebAPI.Models;

namespace Liberty.Reservation.Site.Public.WebAPI.Web.ApiService;

public interface IExternalPublicApiService
{
    Task<HttpResponseModel> GetAsync(
        ExternalService externalService,
        string queryRequest,
        CancellationToken cancellationToken = default
    );

    Task<HttpResponseModel> PostAsync(
        ExternalService externalService,
        string queryRequest,
        string content,
        CancellationToken cancellationToken = default
    );

    Task<HttpResponseModel> PutAsync(
        ExternalService externalService,
        string queryRequest,
        string content,
        CancellationToken cancellationToken = default
    );

    Task<HttpResponseModel> PatchAsync(
        ExternalService externalService,
        string queryRequest,
        string content,
        CancellationToken cancellationToken = default
    );

    Task<HttpResponseModel> DeleteAsync(
        ExternalService externalService,
        string queryRequest,
        CancellationToken cancellationToken = default
    );
}
