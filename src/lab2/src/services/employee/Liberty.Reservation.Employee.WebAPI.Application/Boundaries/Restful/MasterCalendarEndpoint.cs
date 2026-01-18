using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MasterCalendar;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.MasterCalendar;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/master-calendar")]
public class MasterCalendarEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpPost("data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateDataOfDate(
        [FromBody] MasterCalendarEditDataRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new MasterCalendarUpdateDataCommand { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response.AppDate)
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(AppDateAppDateData),
                    response.AppDate.ToString()
                )
            );
    }

    [HttpPost("type")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateTypeOfDate(
        [FromBody] MasterCalendarEditTypeRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new MasterCalendarUpdateTypeCommand { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response.AppDate)
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(AppDateAppDateType),
                    response.AppDate.ToString()
                )
            );
    }

    [HttpDelete("{appDate:long:min(1)}/data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteDataOfDate(
        [FromRoute] long appDate,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new MasterCalendarDeleteDataCommand { Payload = new MasterCalendarDeleteDataRequest(appDate) },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityDeletionAlert(
                    nameof(AppDateAppDateData),
                    response.ToString()
                )
            );
    }

    [HttpDelete("{appDate:long:min(1)}/type")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteTypeOfDate(
        [FromRoute] long appDate,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new MasterCalendarDeleteTypeCommand { Payload = new MasterCalendarDeleteTypeRequest(appDate) },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityDeletionAlert(
                    nameof(AppDateAppDateType),
                    response.ToString()
                )
            );
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<DateOfMasterCalendarResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllDates(
        IPageable pageable,
        [FromQuery] long startDate,
        [FromQuery] long endDate,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new MasterCalendarGetAllQuery(
                pageable,
                startDate,
                endDate
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
