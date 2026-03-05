using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupInventory;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupInventory;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/room-group-inventory")]
public class RoomGroupInventoryEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AdjustAppDatesOfRoomGroupsAsync(
        [FromBody] IEnumerable<RoomGroupChangeRemainRequest> request,
        CancellationToken cancellationToken
    )
    {
        var (addAppDateIds, editAppDateIds) = await Mediator!.Send(
            new RoomGroupInventoryAdjustCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    "RoomGroupInventory",
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
    [ProducesResponseType(typeof(List<RoomGroupAppDateResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAppDatesOfRoomGroups(
        IPageable pageable,
        [FromQuery] long startAppDateId,
        [FromQuery] long endAppDateId,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new RoomGroupInventoryGetAllQuery(
                pageable,
                startAppDateId,
                endAppDateId
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
