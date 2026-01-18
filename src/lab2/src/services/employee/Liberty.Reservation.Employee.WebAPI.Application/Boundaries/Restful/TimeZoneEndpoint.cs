using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.UseCases.Queries.Timezone;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/timezones")]
[AllowAnonymous]
public class TimeZoneEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TimeZoneResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListTimezoneResponse(
        CancellationToken cancellationToken
    )
    {
        var (header, response) = await Mediator!.Send(
            new GetAllTimeZoneQuery(),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(header);
    }
}
