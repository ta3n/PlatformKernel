using Liberty.Reservation.Application.Exceptions;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PlanRoomGroupSiteService(
    ILogger<PlanRoomGroupSiteService> logger,
    IPlanRoomGroupSiteRepository repository
) : BaseServiceRelation<PlanRoomGroupSite>(logger, repository), IPlanRoomGroupSiteService
{
    public async Task<List<PlanRoomGroupSite>> FindAllByPlanIdAndRomTypeIdAsync(
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

    public async Task<PlanRoomGroupSite?> FindByPlanIdAndRomTypeIdAsync(
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
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PlanRoomGroupSite> UpdateMinimumPriceAsync(
        long planId,
        long romGroupId,
        long siteId,
        PlanRoomGroupSite entityToUpdate,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = repository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.PlanId == planId
                    && x.RoomGroupId == romGroupId
                    && x.SiteId == siteId
            );

        var entity = await queryable.SingleOrDefaultAsync(cancellationToken)
            ?? throw new PlanRoomGroupSiteNotFoundException(planId, romGroupId, siteId);
        entity.IsEnabledMinimumPrice = entityToUpdate.IsEnabledMinimumPrice;
        entity.MinimumPrice = entityToUpdate.MinimumPrice;
        return await UpdateAsync(
            entity,
            true,
            cancellationToken
        );
    }
}
