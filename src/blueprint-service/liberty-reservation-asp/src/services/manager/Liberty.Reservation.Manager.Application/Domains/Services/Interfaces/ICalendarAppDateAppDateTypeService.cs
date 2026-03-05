using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface ICalendarAppDateAppDateTypeService : IBaseServiceRelation<CalendarAppDateAppDateType>
{
    Task<(
        List<CalendarAppDateAppDateType> addAppDateOfCalendars,
        List<CalendarAppDateAppDateType> removeAppDateOfCalendars
        )> ChangeCalendarAppDate(
        long calendarId,
        List<CalendarAppDateAppDateType> listCalendarAppDateAppDateType,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );
}
