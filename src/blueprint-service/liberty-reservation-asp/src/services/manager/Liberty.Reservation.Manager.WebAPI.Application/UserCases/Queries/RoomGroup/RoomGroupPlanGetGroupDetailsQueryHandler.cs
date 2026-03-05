using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Plan;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;

public class RoomGroupPlanGetGroupDetailsQueryHandler(
    IMapper mapper,
    IMediator mediator,
    IPlanRoomGroupService planRoomGroupService
) : QuerySingleBaseHandler<RoomGroupPlanGetGroupDetailsQuery, object>(mapper)
{
    protected override async Task<(IHeaderDictionary, object)> HandleAsync(
        RoomGroupPlanGetGroupDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        var planRoomGroup = await planRoomGroupService.FindByRoomGroupIdAsync(
            request.Id,
            cancellationToken
        );

        var response = await mediator.Send(
            new PlanGetGroupDetailsQuery(
                planRoomGroup.PlanId,
                request.Group
            ),
            cancellationToken
        );

        return response;
    }
}
