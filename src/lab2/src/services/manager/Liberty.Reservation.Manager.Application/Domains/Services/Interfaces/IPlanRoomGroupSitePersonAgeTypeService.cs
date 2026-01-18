using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IPlanRoomGroupSitePersonAgeTypeService : IBaseServiceRelation<PlanRoomGroupSitePersonAgeType>
{
    Task<List<PlanRoomGroupSitePersonAgeType>> FindAllByPlanIdAndRomTypeIdAsync(
        long planId,
        long romTypeId,
        long siteId,
        CancellationToken cancellationToken = default
    );
}
