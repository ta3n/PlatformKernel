using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupPrice;

public class RoomGroupPriceGetDiscountQueryHandler(
    IMapper mapper,
    IMediator mediator,
    ISecurityContextAccessor securityContextAccessor,
    IPlanRoomGroupService planRoomGroupService
) : QuerySingleBaseHandler<RoomGroupPriceGetDiscountQuery, PlanRoomDetailDiscountResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, PlanRoomDetailDiscountResponse)> HandleAsync(
        RoomGroupPriceGetDiscountQuery request,
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
            new PlanPriceGetDiscountQuery(
                planForRoomOnly.Id,
                request.RoomTypeId,
                request.SiteId
            ),
            cancellationToken
        );
        return response;
    }
}
