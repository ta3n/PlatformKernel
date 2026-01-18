using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

public class PlanPriceGetMinimumPriceQueryHandler(
    IMapper mapper,
    IPlanRoomGroupSiteRepository planRoomGroupSiteRepository
) : QuerySingleBaseHandler<PlanPriceGetMinimumPriceQuery, PlanRoomSiteMinimumPriceResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, PlanRoomSiteMinimumPriceResponse)> HandleAsync(
        PlanPriceGetMinimumPriceQuery request,
        CancellationToken cancellationToken
    )
    {
        var queryable = planRoomGroupSiteRepository.GetQueryableWithAsNoTracking()
            .Where(p => p.PlanId == request.PlanId)
            .Where(p => p.RoomGroupId == request.RoomId)
            .Where(p => p.SiteId == request.SiteId)
            .ProjectTo<PlanRoomSiteMinimumPriceResponse>(Mapper.ConfigurationProvider);

        var planRoomGroupSiteResponse = await queryable.SingleOrDefaultAsync(cancellationToken)
            ?? new PlanRoomSiteMinimumPriceResponse
            {
                IsEnabledMinimumPrice = false,
                MinimumPrice = null,
                PlanId = request.PlanId,
                RoomId = request.RoomId,
                SiteId = request.SiteId
            };
        return (new HeaderDictionary(), planRoomGroupSiteResponse);
    }
}
