using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services.Implements;

public class AppDateOfRoomService : IAppDateOfRoomService
{
    public List<RoomGroupAppDate> CreateAppDatesOfRoomGroups(
        long roomGroupId,
        List<UpdatePriceData> payload,
        List<RoomGroupAppDate> appDatesOfOptionItems,
        List<AppDate> existingAppDates,
        out List<RoomGroupAppDate> updateAppDatesOfRoomGroups
    )
    {
        var createAppDatesOfRoomGroups = new List<RoomGroupAppDate>();
        updateAppDatesOfRoomGroups = [];

        foreach (var appDate in payload)
        {
            _ = long.TryParse(appDate.AppointedDate, out var appointedDateLong);
            _ = int.TryParse(appDate.StopStartDivision, out var stopStartDivision);
            var existingAppDateOfOptionItem = appDatesOfOptionItems.Find(
                x => x.RoomGroupId == roomGroupId && x.AppDateId == appointedDateLong
            );
            var existingAppDate = existingAppDates.Find(
                x => x.DateTime == AppDate.GetDateTime(appointedDateLong)
            );
            if (existingAppDateOfOptionItem is null)
            {
                var addAppDate = new RoomGroupAppDate
                {
                    RoomGroupId = roomGroupId,
                    IsNotSelled = stopStartDivision == 0
                };

                if (existingAppDate is null)
                {
                    addAppDate.AppDateId = appointedDateLong;
                    addAppDate.AppDate = new()
                    {
                        Id = appointedDateLong,
                        DateTime = AppDate.GetDateTime(appointedDateLong)
                    };
                }
                else
                {
                    addAppDate.AppDateId = existingAppDate.Id;
                }

                createAppDatesOfRoomGroups.Add(addAppDate);
            }
            else
            {
                var editAppDate = existingAppDateOfOptionItem.Clone<RoomGroupAppDate>();
                editAppDate.IsNotSelled = stopStartDivision == 0;

                updateAppDatesOfRoomGroups.Add(editAppDate);
            }
        }

        return createAppDatesOfRoomGroups;
    }
}
