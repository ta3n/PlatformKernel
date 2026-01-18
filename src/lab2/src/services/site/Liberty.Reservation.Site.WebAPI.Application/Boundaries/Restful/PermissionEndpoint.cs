using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Permissions;

namespace Liberty.Reservation.Site.WebAPI.Application.Boundaries.Restful;

[Route("api/permissions")]
public class PermissionEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PermissionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPermission(
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PermissionGetAllQuery(),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
