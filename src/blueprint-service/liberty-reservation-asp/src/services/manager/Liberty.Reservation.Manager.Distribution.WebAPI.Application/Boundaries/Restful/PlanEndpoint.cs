using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Commands.Plan;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Plan;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Web.Rest.Utilities;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Boundaries.Restful;

[Route("api/plan")]
[Produces("application/xml", "application/json")]
[Consumes("application/xml", "application/json")]
public class PlanEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BaseDataResponse<GetPlanResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlan(
        [FromQuery] GetPlanRequest request,
        CancellationToken cancellationToken
    )
    {
        var (_, response) = await Mediator!.Send(
            new PlanGetQuery(request),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNoContent(response, response.Data?.AgtPlanRoomInfos);
    }

    [HttpGet("price-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BaseDataResponse<GetPriceDataPlanRoomResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPriceDataByPlanRoom(
        [FromQuery] GetPriceDataByPlanRoomRequest request,
        CancellationToken cancellationToken
    )
    {
        var (_, response) = await Mediator!.Send(
            new PlanPriceQuery(request),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNoContent(response, response.Data?.TariffData);
    }

    [HttpPut("price-data")]
    [ProducesResponseType(typeof(BaseDataResponse<UpdatePriceDataResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePriceDataByPlanRoom(
        [FromBody] UpdatePlanRoomRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new UpdatePriceDataPlanRoomCommand { Payload = request },
            cancellationToken
        );

        return Ok(response);
    }

    [HttpPatch("publish-accept")]
    [ProducesResponseType(typeof(BaseDataResponse<UpdatePublishAcceptPlanResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePublishAcceptByPlan(
        [FromBody] UpdatePublishAcceptPlanRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanUpdatePublishAcceptCommand(request.ScAgtPlanCode!, request.ScAgtFacilityCode!) { Payload = request },
            cancellationToken
        );

        return Ok(response);
    }
}
