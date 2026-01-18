using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupPrice;

public class RoomGroupPriceGetSaleQueryHandler(
    IMapper mapper,
    IMediator mediator,
    ISecurityContextAccessor securityContextAccessor,
    IPlanRoomGroupService planRoomGroupService
) : QuerySingleBaseHandler<RoomGroupPriceGetSaleQuery, PlanRoomDetailSaleResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, PlanRoomDetailSaleResponse)> HandleAsync(
        RoomGroupPriceGetSaleQuery request,
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
            new PlanPriceGetSaleQuery(
                planForRoomOnly.Id,
                request.RoomTypeId,
                request.SiteId
            ),
            cancellationToken
        );

        return response;
    }
}
