using Liberty.Entity.Utils;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.Web.ApiService;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful.External.ReservationEmployee;

[ApiExplorerSettings(GroupName = "reservation-employee")]
[Route("api/reservation/employee/destinations")]
public class DestinationsEndpoint(
    IMapper mapper,
    IMediator mediator,
    IExternalApiService externalApiService,
    ISecurityContextAccessor securityContextAccessor
) : BaseEndpoint(mapper, mediator)
{
    private const string Endpoint = "api/facilities";

    [HttpGet]
    [ProducesResponseType(typeof(List<DestinationResponse>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetAllDestinations(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var (headers, data) = await externalApiService.GetAsync(
            ExternalService.ReservationEmployeeService,
            $"{Endpoint}/{facilityId}/destinations{HttpContext.Request.QueryString.Value}",
            new Dictionary<string, string> { { "accept-language", LanguageHeaderUtil.GetLanguageCodeFromHeader() } },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(data).WithHeaders(headers);
    }
}
