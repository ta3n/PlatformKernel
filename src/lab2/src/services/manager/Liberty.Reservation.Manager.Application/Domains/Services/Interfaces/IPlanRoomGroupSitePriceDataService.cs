using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IPlanRoomGroupSitePriceDataService
    : IBaseServiceRelation<PlanRoomGroupSitePriceData>
{
    Task<(
        List<PlanRoomGroupSitePriceData> addPriceDataOfSiteInPlanRooms,
        List<PlanRoomGroupSitePriceData> removeDataOfSiteInPlanRooms
        )> ChangeRangePersonOfSiteInPlanRoom(
        long planId,
        long roomTypeId,
        long siteId,
        List<PlanRoomGroupSitePriceData> listPriceDataOfSiteInPlanRoom,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );
}
