using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

public class PlanPriceGetStandardPriceQueryHandler(
    IMapper mapper,
    IPlanRoomGroupSiteAppDateTypePriceDataRepository planRoomGroupSiteRepository,
    IRoomGroupRepository roomGroupRepository
) : QuerySingleBaseHandler<PlanPriceGetStandardPriceQuery, PlanRoomDetailStandardPriceResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, PlanRoomDetailStandardPriceResponse)> HandleAsync(
        PlanPriceGetStandardPriceQuery request,
        CancellationToken cancellationToken
    )
    {
        var queryable = planRoomGroupSiteRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.PlanId == request.PlanId)
            .Where(x => x.RoomGroupId == request.RoomTypeId)
            .Where(x => x.SiteId == request.SiteId)
            .Where(x => x.IsEnabled)
            .Where(x => !x.AppDateType!.IsDeleted)
            .OrderBy(x => x.PriceData!.PersonMin)
            .ProjectTo<RomTypeStandardPrinceDataResponse>(Mapper.ConfigurationProvider);

        var dataOfPrince = await queryable.ToListAsync(
            cancellationToken
        );

        var roomGroup = await roomGroupRepository
                .GetQueryableWithAsNoTracking()
                .Select(
                    x => new
                    {
                        x.Id,
                        x.CapacityMin,
                        x.CapacityMax
                    }
                )
                .SingleOrDefaultAsync(
                    x => x.Id == request.RoomTypeId,
                    cancellationToken
                )
            ?? throw new RoomGroupNotfoundException();

        var data = new PlanRoomDetailStandardPriceResponse(
            request.PlanId,
            request.RoomTypeId,
            request.SiteId,
            roomGroup.CapacityMin,
            roomGroup.CapacityMax,
            dataOfPrince
        );

        return (new HeaderDictionary(), data);
    }
}
