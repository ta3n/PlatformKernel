using System.Data;
using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IPlanRoomGroupSiteAppDateService : IBaseServiceRelation<PlanRoomGroupSiteAppDate>
{
    Task<(
        List<PlanRoomGroupSiteAppDate> addDateOfSiteInPlanRooms,
        List<PlanRoomGroupSiteAppDate> updateDateOfSiteInPlanRooms
        )> ChangeDateDataOfSiteInPlanRoom(
        long planId,
        long roomTypeId,
        long siteId,
        List<PlanRoomGroupSiteAppDate> listAppDateOfSiteInPlanRoom,
        bool autoSave = true,
        bool isAdd = true,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default
    );
}
