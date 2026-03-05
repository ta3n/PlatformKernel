using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PlanRoomGroupSiteAppDateService(
    ILogger<PlanRoomGroupSiteAppDateService> logger,
    IPlanRoomGroupSiteAppDateRepository repository
) : BaseServiceRelation<PlanRoomGroupSiteAppDate>(logger, repository), IPlanRoomGroupSiteAppDateService
{
    public async Task<(
        List<PlanRoomGroupSiteAppDate> addDateOfSiteInPlanRooms,
        List<PlanRoomGroupSiteAppDate> updateDateOfSiteInPlanRooms
        )> ChangeDateDataOfSiteInPlanRoom(
        long planId,
        long roomTypeId,
        long siteId,
        List<PlanRoomGroupSiteAppDate> listAppDateOfSiteInPlanRoom,
        bool autoSave = true,
        bool isAdd = true,
        CancellationToken cancellationToken = default
    )
    {
        var startDate = listAppDateOfSiteInPlanRoom.Select(a => a.DateCalendar).Min();
        var endDate = listAppDateOfSiteInPlanRoom.Select(a => a.DateCalendar).Max();

        var appDateOfSiteInPlanRooms = await FindAllByPlanIdAndRomTypeIdAsync(
            planId,
            roomTypeId,
            siteId,
            startDate,
            endDate,
            cancellationToken
        );

        var comparer = new DelegateEqualityComparer<PlanRoomGroupSiteAppDate>(
            (
                x,
                y
            ) => x!.DateCalendar == y?.DateCalendar,
            obj => obj.DateCalendar.GetHashCode()
        );

        var updateDateOfSiteInPlanRooms = appDateOfSiteInPlanRooms
            .GroupBy(item => item, comparer)
            .SelectMany(
                group =>
                    group.Take(
                        listAppDateOfSiteInPlanRoom.Count(
                            updateItem => comparer.Equals(updateItem, group.Key)
                        )
                    )
            )
            .ToList();

        var addDateOfSiteInPlanRooms = listAppDateOfSiteInPlanRoom
            .GroupBy(item => item, comparer)
            .SelectMany(
                group =>
                    group.Skip(
                        updateDateOfSiteInPlanRooms.Count(
                            updateItem => comparer.Equals(updateItem, group.Key)
                        )
                    )
            )
            .ToList();

        foreach (var dateData in updateDateOfSiteInPlanRooms)
        {
            var matchingDate = listAppDateOfSiteInPlanRoom.Find(p => comparer.Equals(p, dateData));
            if (matchingDate is not null)
            {
                dateData.UseAutoDiscount = matchingDate.UseAutoDiscount;
                dateData.PointRate = matchingDate.PointRate;
                listAppDateOfSiteInPlanRoom.Remove(matchingDate);
            }
        }

        if (addDateOfSiteInPlanRooms.Count > 0)
        {
            _ = await CreateRangeAsync(addDateOfSiteInPlanRooms, autoSave, cancellationToken);
        }

        if (updateDateOfSiteInPlanRooms.Count > 0)
        {
            _ = await UpdateRangeAsync(updateDateOfSiteInPlanRooms, autoSave, cancellationToken);
        }

        return (isAdd ? addDateOfSiteInPlanRooms : [], updateDateOfSiteInPlanRooms);
    }

    public async Task<List<PlanRoomGroupSiteAppDate>> FindAllByPlanIdAndRomTypeIdAsync(
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
