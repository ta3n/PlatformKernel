using Liberty.Reservation.User.WebAPI.Application.Models.Responses;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.SystemConfig;

namespace Liberty.Reservation.User.WebAPI.Application.Boundaries.Restful;

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
