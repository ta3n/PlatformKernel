using Liberty.Cache.Services;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.SystemConfig;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.SystemConfig;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/system-config")]
[ApiVersion(1)]
public class SystemConfigEndpoint(
    IMapper mapper,
    IMediator mediator,
    ICacheService cacheService
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(SystemConfigResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSystemConfig(
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(new SystemConfigGetQuery(), cancellationToken);
        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPut("can-online-payment")]
    [ProducesResponseType(typeof(SystemConfigResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangeCanOnlinePayment(
        SystemConfigCanOnlinePaymentRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(new SystemConfigCanOnlinePaymentCommand { Payload = request }, cancellationToken);
        await cacheService.RemoveByPatternsAsync(true, CacheKeys.ResetPatternManagerFacility);
        return ActionResultUtil.WrapOrNotFound(response);
    }
}
