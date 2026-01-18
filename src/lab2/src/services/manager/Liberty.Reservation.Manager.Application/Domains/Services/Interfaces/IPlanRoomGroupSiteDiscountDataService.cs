using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IPlanRoomGroupSiteDiscountDataService : IBaseServiceRelation<PlanRoomGroupSiteDiscountData>
{
    Task<(
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
    );
}
