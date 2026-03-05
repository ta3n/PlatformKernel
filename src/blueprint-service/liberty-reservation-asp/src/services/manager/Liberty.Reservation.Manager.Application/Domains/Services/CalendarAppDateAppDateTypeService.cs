using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class CalendarAppDateAppDateTypeService(
    ILogger<CalendarAppDateAppDateTypeService> logger,
    ICalendarAppDateAppDateTypeRepository repository
)
    : BaseServiceRelation<CalendarAppDateAppDateType>(logger, repository),
        ICalendarAppDateAppDateTypeService
{
    private async Task<List<CalendarAppDateAppDateType>> GetCalendarAppDateByCalendarId(
        long calendarId,
        List<int> dateCalendars,
        CancellationToken cancellationToken = default
    )
    {
        return await repository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.CalendarId == calendarId)
            .Where(x => dateCalendars.Contains(x.DateCalendar))
            .ToListAsync(cancellationToken);
    }

    public async Task<(
        List<CalendarAppDateAppDateType> addAppDateOfCalendars,
        List<CalendarAppDateAppDateType> removeAppDateOfCalendars
        )> ChangeCalendarAppDate(
        long calendarId,
        List<CalendarAppDateAppDateType> listCalendarAppDateAppDateType,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var listDateCalendar = listCalendarAppDateAppDateType
            .Select(x => x.DateCalendar)
            .ToList();

        var appDateOfCalendars = await GetCalendarAppDateByCalendarId(
            calendarId,
            listDateCalendar,
            cancellationToken
        );

        var comparer = new DelegateEqualityComparer<CalendarAppDateAppDateType>(
            (
                    x,
                    y
                ) =>
                x?.AppDateTypeId == y?.AppDateTypeId
                && x!.DateCalendar == y?.DateCalendar
                && x!.IsDeleted == y.IsDeleted,
            obj =>
                obj.AppDateTypeId.GetHashCode()
                ^ obj.DateCalendar.GetHashCode()
                ^ obj.IsDeleted.GetHashCode()
        );

        var filteredListAdd = listCalendarAppDateAppDateType
            .Where(x => !x.IsDeleted)
            .ToList();

        var addAppDateOfCalendars = filteredListAdd
            .Except(appDateOfCalendars, comparer)
            .ToList();

        var removeAppDateOfCalendars = appDateOfCalendars
            .Except(listCalendarAppDateAppDateType, comparer)
            .ToList();

        if (addAppDateOfCalendars.Count > 0)
        {
            _ = await CreateRangeAsync(addAppDateOfCalendars, autoSave, cancellationToken);
        }

        if (removeAppDateOfCalendars.Count > 0)
        {
            _ = await DeleteRangeAsync(removeAppDateOfCalendars, autoSave, cancellationToken);
        }

        return (addAppDateOfCalendars, removeAppDateOfCalendars);
    }
}
