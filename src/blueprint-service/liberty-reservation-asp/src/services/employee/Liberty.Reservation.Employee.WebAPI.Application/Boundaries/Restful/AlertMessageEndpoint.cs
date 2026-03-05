using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.AlertMessage;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/alert-message")]
public class AlertMessageEndpoint(
    IMapper mapper,
    IMediator mediator,
    IAlertMessageService alertMessageService
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(AlertMessageResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAlertMessage(
        CancellationToken cancellationToken
    )
    {
        var alertMessage = await alertMessageService.GetFirstAlertMessageAsync(cancellationToken);
        var response = Mapper.Map<AlertMessageResponse>(alertMessage);
        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateAlertMessage(
        [FromBody] AlertMessageUpdateRequest request,
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await Mediator!.Send(
            new AlertMessageUpdateCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(AlertMessage),
                    response.ToString()
                )
            );
    }
}
