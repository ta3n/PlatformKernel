using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupInventory;

public class RoomGroupInventoryGetAllQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IRoomGroupAppDateRepository roomGroupAppDateRepository
) : QueryPageBaseHandler<RoomGroupInventoryGetAllQuery, RoomGroupAppDateResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<RoomGroupAppDateResponse>)> HandleAsync(
        RoomGroupInventoryGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = roomGroupAppDateRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.RoomGroup)
            .Where(
                x => x.RoomGroup!.FacilityRoomGroups!.Any(
                    t => t.FacilityId == facilityId
                )
            )
            .Where(
                x => x.AppDateId >= request.StartAppDate
                    && x.AppDateId <= request.EndAppDate
            )
            .ProjectTo<RoomGroupAppDateResponse>(Mapper.ConfigurationProvider);

        var page = await queryable.UsePageableAsync(
            request.Pageable,
            cancellationToken: cancellationToken
        );

        var headers = page.GeneratePaginationHttpHeaders();
        var data = page.Content.ToList();
        foreach (var value in data)
        {
            value.RemainNumber = value.SellNumber - value.ReservedNumber;
        }

        return (headers, data);
    }
}
