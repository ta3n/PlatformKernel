using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.OptionItemInventory;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.OptionItemInventory;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/option-item-inventory")]
public class OptionItemInventoryEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AdjustAppDatesOfOptionItems(
        [FromBody] IEnumerable<OptionItemChangeRemainRequest> request,
        CancellationToken cancellationToken
    )
    {
        var (addAppDateIds, editAppDateIds) = await Mediator!.Send(
            new OptionItemInventoryAdjustCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    "OptionItemInventory",
                    JsonConvert.SerializeObject(
                        new
                        {
                            addAppDateIds,
                            editAppDateIds
                        }
                    )
                )
            );
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<OptionItemAppDateResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAppDatesOfOptionItems(
        IPageable pageable,
        [FromQuery] long startAppDateId,
        [FromQuery] long endAppDateId,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new OptionItemInventoryGetAllQuery(
                pageable,
                startAppDateId,
                endAppDateId
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
