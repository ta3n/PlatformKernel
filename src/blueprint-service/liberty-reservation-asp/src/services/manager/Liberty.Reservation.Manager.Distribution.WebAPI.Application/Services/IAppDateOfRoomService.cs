using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services;

public interface IAppDateOfRoomService
{
    List<RoomGroupAppDate> CreateAppDatesOfRoomGroups(
        long roomGroupId,
        List<UpdatePriceData> payload,
        List<RoomGroupAppDate> appDatesOfOptionItems,
        List<AppDate> existingAppDates,
        out List<RoomGroupAppDate> updateAppDatesOfRoomGroups
    );
}
