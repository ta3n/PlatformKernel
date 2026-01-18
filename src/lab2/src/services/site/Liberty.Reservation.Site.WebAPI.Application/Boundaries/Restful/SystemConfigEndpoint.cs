using Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.SystemConfig;
using Microsoft.AspNetCore.Authorization;

namespace Liberty.Reservation.Site.WebAPI.Application.Boundaries.Restful;

[AllowAnonymous]
[Route("api/system-config")]
[ApiVersion(1)]
public class SystemConfigEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(SystemConfigResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSystemConfig(
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new SystemConfigGetQuery(),
            cancellationToken
        );
        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
