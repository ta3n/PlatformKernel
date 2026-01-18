using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.Permissions;

namespace Liberty.Reservation.User.WebAPI.Application.Boundaries.Restful;

[Route("api/permission")]
public class PermissionEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PermissionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPermissionConfig(
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
