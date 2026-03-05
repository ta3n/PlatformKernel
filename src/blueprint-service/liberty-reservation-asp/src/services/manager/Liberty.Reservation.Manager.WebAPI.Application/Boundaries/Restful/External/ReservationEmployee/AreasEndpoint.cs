using Liberty.Entity.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.Web.ApiService;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful.External.ReservationEmployee;

[ApiExplorerSettings(GroupName = "reservation-employee")]
[Route("api/reservation/employee/areas")]
public class AreasEndpoint(
    IMapper mapper,
    IMediator mediator,
    IExternalApiService externalApiService
) : BaseEndpoint(mapper, mediator)
{
    private const string Endpoint = "api/areas";

    [HttpGet]
    [ProducesResponseType(typeof(List<AreaResponse>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetAllDestinations(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var (headers, data) = await externalApiService.GetAsync(
            ExternalService.ReservationEmployeeService,
            $"{Endpoint}{HttpContext.Request.QueryString.Value}",
            new Dictionary<string, string> { { "accept-language", LanguageHeaderUtil.GetLanguageCodeFromHeader() } },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(data).WithHeaders(headers);
    }
}
