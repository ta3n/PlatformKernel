using Liberty.Reservation.Site.Public.WebAPI.Web.ApiService;
using Microsoft.AspNetCore.Authorization;

namespace Liberty.Reservation.Site.Public.WebAPI.Boundaries.Restful;

[AllowAnonymous]
[Route("api/booking")]
public partial class BookingEndpoint(
    IExternalPublicApiService externalApiService
) : BaseEndpoint(externalApiService);
