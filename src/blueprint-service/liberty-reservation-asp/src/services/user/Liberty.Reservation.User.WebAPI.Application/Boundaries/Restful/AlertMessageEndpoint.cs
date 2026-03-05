using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.UseCases.Queries.AlertMessage;
using Liberty.Reservation.User.Application.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Liberty.Reservation.User.WebAPI.Application.Boundaries.Restful;

[Route("api/alert-message")]
public class AlertMessageEndpoint(
    IMapper mapper,
    IMediator mediator,
    ISecurityContextAccessor securityContextAccessor
) : BaseEndpoint(mapper, mediator)
{
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlertMessageResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListActiveAlertMessage(
        CancellationToken cancellationToken
    )
    {
        var (header, response) = await Mediator!.Send(
            new GetAllAlertMessageActiveQuery(securityContextAccessor.GetLanguageCode() ?? "ja-JP"),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(header);
    }
}
