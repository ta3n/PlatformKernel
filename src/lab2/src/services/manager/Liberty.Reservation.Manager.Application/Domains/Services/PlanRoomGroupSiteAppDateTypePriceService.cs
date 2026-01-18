using Liberty.ApplicationShared.Utils;
using IPlanRoomGroupSiteAppDateTypePriceDataRepository =
    Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces.IPlanRoomGroupSiteAppDateTypePriceDataRepository;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PlanRoomGroupSiteAppDateTypePriceService(
    ILogger<PlanRoomGroupSiteAppDateTypePriceService> logger,
    IPlanRoomGroupSiteAppDateTypePriceDataRepository repository
)
    : BaseServiceRelation<PlanRoomGroupSiteAppDateTypePriceData>(logger, repository),
        IPlanRoomGroupSiteAppDateTypePriceService
{
    private async Task<
        List<PlanRoomGroupSiteAppDateTypePriceData>
    > FindAllByPlanIdAndRomTypeIdAsync(
        long planId,
        long roomTypeId,
        long siteId,
        CancellationToken cancellationToken = default
    )
    {
        return await repository
            .GetQueryable()
            .Include(x => x.PriceData)
            .Where(x => !x.AppDateType!.IsDeleted)
            .Where(x => x.PlanId == planId && x.RoomGroupId == roomTypeId && x.SiteId == siteId)
            .ToListAsync(cancellationToken);
    }

    public async Task<(
        List<PlanRoomGroupSiteAppDateTypePriceData> addPriceDataOfSiteInPlanRooms,
        List<PlanRoomGroupSiteAppDateTypePriceData> updatePriceDataOfSiteInPlanRooms,
        List<PlanRoomGroupSiteAppDateTypePriceData> removeDataOfSiteInPlanRooms
        )> ChangePriceDataOfSiteInPlanRoom(
        long planId,
        long roomTypeId,
        long siteId,
        List<PlanRoomGroupSiteAppDateTypePriceData> listPriceDataOfSiteInPlanRoom,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var priceDataOfSiteInPlanRooms = await FindAllByPlanIdAndRomTypeIdAsync(
            planId,
            roomTypeId,
            siteId,
            cancellationToken
        );

        var comparer = new DelegateEqualityComparer<PlanRoomGroupSiteAppDateTypePriceData>(
            (
                    x,
                    y
                ) =>
                x?.AppDateTypeId == y?.AppDateTypeId
                && x!.PriceData!.PersonMin == y?.PriceData!.PersonMin
                && x.PriceData.PersonMax == y?.PriceData!.PersonMax,
            obj =>
                obj.AppDateTypeId.GetHashCode()
                ^ obj.PriceData!.PersonMin.GetHashCode()
                ^ obj.PriceData.PersonMax.GetHashCode()
        );

        var removeDataOfSiteInPlanRooms = priceDataOfSiteInPlanRooms
            .GroupBy(item => item, comparer)
            .SelectMany(
                group =>
                    group.Skip(
                        listPriceDataOfSiteInPlanRoom.Count(
                            updateItem => comparer.Equals(updateItem, group.Key)
                        )
                    )
            )
            .ToList();

        var updatePriceDataOfSiteInPlanRooms = priceDataOfSiteInPlanRooms
            .GroupBy(item => item, comparer)
            .SelectMany(
                group =>
                    group.Take(
                        listPriceDataOfSiteInPlanRoom.Count(
                            updateItem => comparer.Equals(updateItem, group.Key)
                        )
                    )
            )
            .ToList();

        var addPriceDataOfSiteInPlanRooms = listPriceDataOfSiteInPlanRoom
            .GroupBy(item => item, comparer)
            .SelectMany(
                group =>
                    group.Skip(
                        updatePriceDataOfSiteInPlanRooms.Count(
                            updateItem => comparer.Equals(updateItem, group.Key)
                        )
                    )
            )
            .ToList();

        foreach (var priceData in updatePriceDataOfSiteInPlanRooms)
        {
            var matchingPrice = listPriceDataOfSiteInPlanRoom.Find(
                p => comparer.Equals(p, priceData)
            );
            if (matchingPrice is null)
            {
                continue;
            }

            priceData.PriceData!.Price = matchingPrice.PriceData!.Price;
            listPriceDataOfSiteInPlanRoom.Remove(matchingPrice);
        }

        if (addPriceDataOfSiteInPlanRooms is { Count: > 0 })
        {
            _ = await CreateRangeAsync(addPriceDataOfSiteInPlanRooms, autoSave, cancellationToken);
        }

        if (updatePriceDataOfSiteInPlanRooms is { Count: > 0 })
        {
            _ = await UpdateRangeAsync(
                updatePriceDataOfSiteInPlanRooms,
                autoSave,
                cancellationToken
            );
        }

        if (removeDataOfSiteInPlanRooms is not { Count: > 0 })
        {
            return (
                addPriceDataOfSiteInPlanRooms,
                updatePriceDataOfSiteInPlanRooms,
                removeDataOfSiteInPlanRooms
            );
        }

        await DeleteRangeAsync(
            removeDataOfSiteInPlanRooms,
            autoSave,
            cancellationToken
        );

        return (
            addPriceDataOfSiteInPlanRooms,
            updatePriceDataOfSiteInPlanRooms,
            removeDataOfSiteInPlanRooms
        );
    }
}
