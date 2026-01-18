namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PlanRoomGroupSitePersonAgeTypeService(
    ILogger<PlanRoomGroupSitePersonAgeTypeService> logger,
    IPlanRoomGroupSitePersonAgeTypeRepository repository
) : BaseServiceRelation<PlanRoomGroupSitePersonAgeType>(logger, repository), IPlanRoomGroupSitePersonAgeTypeService
{
    public async Task<List<PlanRoomGroupSitePersonAgeType>> FindAllByPlanIdAndRomTypeIdAsync(
        long planId,
        long romTypeId,
        long siteId,
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
            .ToListAsync(cancellationToken);
    }
}
