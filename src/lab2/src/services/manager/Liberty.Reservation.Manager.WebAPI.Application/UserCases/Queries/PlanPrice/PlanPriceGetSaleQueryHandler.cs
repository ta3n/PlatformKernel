using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Exceptions;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

public class PlanPriceGetSaleQueryHandler(
    IMapper mapper,
    IPlanRoomGroupSiteRepository planRoomGroupSiteRepository
) : QuerySingleBaseHandler<PlanPriceGetSaleQuery, PlanRoomDetailSaleResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, PlanRoomDetailSaleResponse)> HandleAsync(
        PlanPriceGetSaleQuery request,
        CancellationToken cancellationToken
    )
    {
        var queryable = planRoomGroupSiteRepository
            .GetQueryableWithAsNoTracking()
            .Where(a => a.PlanId == request.PlanId)
            .Where(a => a.RoomGroupId == request.RomTypeId)
            .Where(a => a.SiteId == request.SiteId)
            .ProjectTo<PlanRoomDetailSaleResponse>(Mapper.ConfigurationProvider);

        var data = await queryable.FirstOrDefaultAsync(
                cancellationToken
            )
            ?? throw new PlanNotfoundException();

        return (new HeaderDictionary(), data);
    }
}
