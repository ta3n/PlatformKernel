using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PlanRoomGroupSiteAppDatePriceDataService(
    ILogger<PlanRoomGroupSiteAppDatePriceDataService> logger,
    IPlanRoomGroupSiteAppDatePriceDataRepository repository
) : BaseServiceRelation<PlanRoomGroupSiteAppDatePriceData>(logger, repository),
    IPlanRoomGroupSiteAppDatePriceDataService
{
    public async Task<(
        List<PlanRoomGroupSiteAppDatePriceData> addPriceOfSiteInPlanRooms,
        List<PlanRoomGroupSiteAppDatePriceData> updatePriceOfSiteInPlanRooms
        )> ChangePriceDataOfSiteInPlanRoom(
        long planId,
        long roomTypeId,
        long siteId,
        List<PlanRoomGroupSiteAppDatePriceData> listPriceOfSiteInPlanRoom,
        bool autoSave = true,
        bool isAdd = true,
        CancellationToken cancellationToken = default
    )
    {
        var startDate = listPriceOfSiteInPlanRoom.Select(a => a.DateCalendar).Min();
        var endDate = listPriceOfSiteInPlanRoom.Select(a => a.DateCalendar).Max();

        var priceOfSiteInPlanRooms = await FindAllByPlanIdAndRomTypeIdAsync(
            planId,
            roomTypeId,
            siteId,
            startDate,
            endDate,
            cancellationToken
        );

        var comparer = new DelegateEqualityComparer<PlanRoomGroupSiteAppDatePriceData>(
            (
                    x,
                    y
                ) =>
                x!.DateCalendar == y?.DateCalendar
                && x.PriceData!.PersonMin == y.PriceData!.PersonMin
                && x.PriceData!.PersonMax == y.PriceData!.PersonMax,
            obj =>
                obj.DateCalendar.GetHashCode()
                ^ obj.PriceData!.PersonMin.GetHashCode()
                ^ obj.PriceData!.PersonMax.GetHashCode()
        );

        var updatePriceOfSiteInPlanRooms = priceOfSiteInPlanRooms
            .GroupBy(item => item, comparer)
            .SelectMany(
                group =>
                    group.Take(
                        listPriceOfSiteInPlanRoom.Count(
                            updateItem => comparer.Equals(updateItem, group.Key)
                        )
                    )
            )
            .ToList();

        var addPriceOfSiteInPlanRooms = listPriceOfSiteInPlanRoom
            .GroupBy(item => item, comparer)
            .SelectMany(
                group =>
                    group.Skip(
                        updatePriceOfSiteInPlanRooms.Count(
                            updateItem => comparer.Equals(updateItem, group.Key)
                        )
                    )
            )
            .ToList();

        foreach (var priceData in updatePriceOfSiteInPlanRooms)
        {
            var matchingPrice = listPriceOfSiteInPlanRoom.Find(p => comparer.Equals(p, priceData));
            if (matchingPrice is not null)
            {
                priceData.PriceData!.Price = matchingPrice.PriceData!.Price;
                listPriceOfSiteInPlanRoom.Remove(matchingPrice);
            }
        }

        if (addPriceOfSiteInPlanRooms.Count > 0 && isAdd)
        {
            _ = await CreateRangeAsync(addPriceOfSiteInPlanRooms, autoSave, cancellationToken);
        }

        if (updatePriceOfSiteInPlanRooms.Count > 0)
        {
            _ = await UpdateRangeAsync(updatePriceOfSiteInPlanRooms, autoSave, cancellationToken);
        }

        return (
            isAdd ? addPriceOfSiteInPlanRooms : [],
            updatePriceOfSiteInPlanRooms
        );
    }

    private async Task<List<PlanRoomGroupSiteAppDatePriceData>> FindAllByPlanIdAndRomTypeIdAsync(
        long planId,
        long romTypeId,
        long siteId,
        long startDate,
        long endDate,
        CancellationToken cancellationToken = default
    )
    {
        return await repository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.PriceData)
            .Where(
                x => x.PlanId == planId
                    && x.RoomGroupId == romTypeId
                    && x.SiteId == siteId
            )
            .Where(
                x => x.DateCalendar >= startDate
                    && x.DateCalendar <= endDate
            )
            .ToListAsync(cancellationToken);
    }
}
