using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

public class PlanPriceGetRangePersonQueryHandler(
    IMapper mapper,
    IPlanRoomGroupSitePriceDataRepository planRoomGroupSitePriceDataRepository
) : QuerySingleBaseHandler<PlanPriceGetRangePersonQuery, PlanRoomSiteRangePersonResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, PlanRoomSiteRangePersonResponse)> HandleAsync(
        PlanPriceGetRangePersonQuery request,
        CancellationToken cancellationToken
    )
    {
        var queryable = planRoomGroupSitePriceDataRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.PlanId == request.PlanId)
            .Where(x => x.RoomGroupId == request.RoomTypeId)
            .Where(x => x.SiteId == request.SiteId)
            .OrderBy(x => x.PersonMin)
            .ProjectTo<RangePersons>(Mapper.ConfigurationProvider);

        var rangePersons = await queryable.ToListAsync(cancellationToken);

        var data = new PlanRoomSiteRangePersonResponse(
            request.PlanId,
            request.RoomTypeId,
            request.SiteId,
            rangePersons
        );

        return (new HeaderDictionary(), data);
    }
}
