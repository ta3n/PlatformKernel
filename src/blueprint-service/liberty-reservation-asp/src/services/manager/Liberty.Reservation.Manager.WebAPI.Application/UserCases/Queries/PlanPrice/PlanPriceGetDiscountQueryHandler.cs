using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

public class PlanPriceGetDiscountQueryHandler(
    IMapper mapper,
    IPlanRoomGroupSiteDiscountDataRepository planRoomGroupSiteDiscountRepository
) : QuerySingleBaseHandler<PlanPriceGetDiscountQuery, PlanRoomDetailDiscountResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, PlanRoomDetailDiscountResponse)> HandleAsync(
        PlanPriceGetDiscountQuery request,
        CancellationToken cancellationToken
    )
    {
        var queryable = planRoomGroupSiteDiscountRepository
            .GetQueryableWithAsNoTracking()
            .Where(a => a.PlanId == request.PlanId)
            .Where(a => a.RoomGroupId == request.RomTypeId)
            .Where(a => a.SiteId == request.SiteId)
            .ProjectTo<RoomTypeDiscountDataResponse>(Mapper.ConfigurationProvider);

        var dataOfPrince = await queryable.ToListAsync(
            cancellationToken
        );

        var data = new PlanRoomDetailDiscountResponse(
            request.PlanId,
            request.RomTypeId,
            request.SiteId,
            dataOfPrince
        );

        return (new HeaderDictionary(), data);
    }
}
