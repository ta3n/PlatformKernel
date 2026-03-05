using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IPlanRoomGroupSiteAppDatePriceDataService : IBaseServiceRelation<PlanRoomGroupSiteAppDatePriceData>
{
    Task<(
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
    );
}
