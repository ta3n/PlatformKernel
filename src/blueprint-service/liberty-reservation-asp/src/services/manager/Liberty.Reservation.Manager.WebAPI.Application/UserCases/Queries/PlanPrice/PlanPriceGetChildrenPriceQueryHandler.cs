using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

public class PlanPriceGetChildrenPriceQueryHandler(
    IMapper mapper,
    IPlanRoomGroupSitePersonAgeTypeRepository planRoomGroupSitePersonAgeTypeRepository
) : QuerySingleBaseHandler<PlanPriceGetChildrenPriceQuery, PlanRoomDetailChildrenPriceResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, PlanRoomDetailChildrenPriceResponse)> HandleAsync(
        PlanPriceGetChildrenPriceQuery request,
        CancellationToken cancellationToken
    )
    {
        var queryable = planRoomGroupSitePersonAgeTypeRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.PlanId == request.PlanId)
            .Where(x => x.RoomGroupId == request.RomTypeId)
            .Where(x => x.SiteId == request.SiteId)
            .Where(x => x.PersonAgeType!.IsEnabled && x.PersonAgeType!.IsVisible)
            .ProjectTo<RoomTypeChildrenPersonAgeTypeResponse>(Mapper.ConfigurationProvider)
            .OrderBy(x => x.DisplayOrder);

        var listPersonAgeTypes = await queryable.ToListAsync(
            cancellationToken
        );

        var personAgeTypeResponse = listPersonAgeTypes
            .OrderBy(x => x.DisplayOrder)
            .ThenByDescending(x => x.IsMain)
            .ThenBy(x => x.PersonAgeTypeId)
            .ToList();

        var data = new PlanRoomDetailChildrenPriceResponse(
            request.PlanId,
            request.RomTypeId,
            request.SiteId,
            personAgeTypeResponse
        );

        return (new HeaderDictionary(), data);
    }
}
