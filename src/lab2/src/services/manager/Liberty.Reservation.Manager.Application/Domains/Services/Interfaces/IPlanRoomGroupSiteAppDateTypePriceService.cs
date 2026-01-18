using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IPlanRoomGroupSiteAppDateTypePriceService
    : IBaseServiceRelation<PlanRoomGroupSiteAppDateTypePriceData>
{
    Task<(
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
    );
}
