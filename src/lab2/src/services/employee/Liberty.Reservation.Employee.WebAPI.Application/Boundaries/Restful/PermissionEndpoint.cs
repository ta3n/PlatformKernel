using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Permission;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Permissions;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/permission")]
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

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreatePermission(
        [FromBody] PermissionCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PermissionCreateCommand { Payload = request },
            cancellationToken
        );
        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPatch("enable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> EnablePermission(
        [FromBody] IEnumerable<PermissionEnableRequest> request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PermissionEnableCommand { Payload = request },
            cancellationToken
        );
        return ActionResultUtil.WrapOrNotFound(response);
    }
}
