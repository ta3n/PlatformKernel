using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PlanRoomGroupSiteDiscountDataService(
    ILogger<PlanRoomGroupSiteDiscountDataService> logger,
    IPlanRoomGroupSiteDiscountDataRepository repository
)
    : BaseServiceRelation<PlanRoomGroupSiteDiscountData>(logger, repository),
        IPlanRoomGroupSiteDiscountDataService
{
    public async Task<(
        List<PlanRoomGroupSiteDiscountData> addDiscountOfSiteInPlanRooms,
        List<PlanRoomGroupSiteDiscountData> updateDiscountOfSiteInPlanRooms,
        List<PlanRoomGroupSiteDiscountData> removeDiscountOfSiteInPlanRooms
        )> ChangeDiscountDataOfSiteInPlanRoom(
        long planId,
        long roomTypeId,
        long siteId,
        List<PlanRoomGroupSiteDiscountData> listDiscountOfSiteInPlanRoom,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var discountOfSiteInPlanRooms = await FindAllByPlanIdAndRomTypeIdAsync(
            planId,
            roomTypeId,
            siteId,
            cancellationToken
        );

        var comparer = new DelegateEqualityComparer<PlanRoomGroupSiteDiscountData>(
            (
                    x,
                    y
                ) =>
                x!.DiscountData!.StartPrevDay == y?.DiscountData!.StartPrevDay
                && x.DiscountData!.EndPrevDay == y?.DiscountData!.EndPrevDay
                && x.DiscountData!.PersonMin == y?.DiscountData!.PersonMin
                && x.DiscountData!.PersonMax == y?.DiscountData!.PersonMax,
            obj =>
                obj.DiscountData!.StartPrevDay.GetHashCode()
                ^ obj.DiscountData!.EndPrevDay.GetHashCode()
                ^ obj.DiscountData!.PersonMin.GetHashCode()
                ^ obj.DiscountData!.PersonMax.GetHashCode()
        );

        var removeDataOfSiteInPlanRooms = discountOfSiteInPlanRooms
            .GroupBy(item => item, comparer)
            .SelectMany(
                group =>
                    group.Skip(
                        listDiscountOfSiteInPlanRoom.Count(
                            updateItem => comparer.Equals(updateItem, group.Key)
                        )
                    )
            )
            .ToList();

        var updateDiscountOfSiteInPlanRooms = discountOfSiteInPlanRooms
            .GroupBy(item => item, comparer)
            .SelectMany(
                group =>
                    group.Take(
                        listDiscountOfSiteInPlanRoom.Count(
                            updateItem => comparer.Equals(updateItem, group.Key)
                        )
                    )
            )
            .ToList();

        var addDiscountOfSiteInPlanRooms = listDiscountOfSiteInPlanRoom
            .GroupBy(item => item, comparer)
            .SelectMany(
                group =>
                    group.Skip(
                        updateDiscountOfSiteInPlanRooms.Count(
                            updateItem => comparer.Equals(updateItem, group.Key)
                        )
                    )
            )
            .ToList();

        foreach (var priceData in updateDiscountOfSiteInPlanRooms)
        {
            var matchingPrice = listDiscountOfSiteInPlanRoom.Find(
                p => comparer.Equals(p, priceData)
            );
            if (matchingPrice is not null)
            {
                priceData.DiscountData!.PriceSettingType = matchingPrice
                    .DiscountData!
                    .PriceSettingType;
                priceData.DiscountData!.Value = matchingPrice.DiscountData!.Value;

                listDiscountOfSiteInPlanRoom.Remove(matchingPrice);
            }
        }

        if (addDiscountOfSiteInPlanRooms.Count > 0)
        {
            _ = await CreateRangeAsync(addDiscountOfSiteInPlanRooms, autoSave, cancellationToken);
        }

        if (updateDiscountOfSiteInPlanRooms.Count > 0)
        {
            _ = await UpdateRangeAsync(
                updateDiscountOfSiteInPlanRooms,
                autoSave,
                cancellationToken
            );
        }

        if (removeDataOfSiteInPlanRooms.Count > 0)
        {
            _ = await DeleteRangeAsync(removeDataOfSiteInPlanRooms, autoSave, cancellationToken);
        }

        return (
            addDiscountOfSiteInPlanRooms,
            updateDiscountOfSiteInPlanRooms,
            removeDataOfSiteInPlanRooms
        );
    }

    private async Task<List<PlanRoomGroupSiteDiscountData>> FindAllByPlanIdAndRomTypeIdAsync(
        long planId,
        long roomTypeId,
        long siteId,
        CancellationToken cancellationToken = default
    )
    {
        return await repository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.DiscountData)
            .Where(
                x => x.PlanId == planId
                    && x.RoomGroupId == roomTypeId
                    && x.SiteId == siteId
            )
            .ToListAsync(cancellationToken);
    }
}
