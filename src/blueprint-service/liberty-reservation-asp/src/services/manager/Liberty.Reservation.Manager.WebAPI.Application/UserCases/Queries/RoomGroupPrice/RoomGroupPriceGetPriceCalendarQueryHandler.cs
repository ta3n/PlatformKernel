using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupPrice;

public class RoomGroupPriceGetPriceCalendarQueryHandler(
    IMapper mapper,
    IMediator mediator,
    ISecurityContextAccessor securityContextAccessor,
    IPlanRoomGroupService planRoomGroupService
) : QuerySingleBaseHandler<RoomGroupPriceGetPriceCalendarQuery, PlanRoomDetailPriceCalendarResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, PlanRoomDetailPriceCalendarResponse)> HandleAsync(
        RoomGroupPriceGetPriceCalendarQuery request,
        CancellationToken cancellationToken
    )
    {
        var planForRoomOnly = await planRoomGroupService.GetPlanWithRoomOnlyTypeAsync(
                request.RoomTypeId,
                securityContextAccessor.FacilityKey,
                cancellationToken
            )
            ?? throw new RoomOnlyTypeOfPlanNotfoundException();

        var response = await mediator.Send(
            new PlanPriceGetPriceCalendarQuery(
                planForRoomOnly.Id,
                request.RoomTypeId,
                request.SiteId,
                request.StartDate,
                request.EndDate
            ),
            cancellationToken
        );

        return response;
    }
}
