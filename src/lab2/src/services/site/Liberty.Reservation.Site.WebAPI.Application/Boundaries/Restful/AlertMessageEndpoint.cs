using Liberty.Reservation.Application.UseCases.Queries.AlertMessage;
using Microsoft.AspNetCore.Authorization;

namespace Liberty.Reservation.Site.WebAPI.Application.Boundaries.Restful;

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
            new GetAllAlertMessageActiveQuery(securityContextAccessor.GetLanguageCode()),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(header);
    }
}
