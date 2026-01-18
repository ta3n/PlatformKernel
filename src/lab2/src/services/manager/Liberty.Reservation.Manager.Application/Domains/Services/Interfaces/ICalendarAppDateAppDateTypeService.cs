using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

/// <summary>
/// Service interface for managing the relationship between CalendarAppDate and AppDateType entities.
/// </summary>
public interface ICalendarAppDateAppDateTypeService : IBaseServiceRelation<CalendarAppDateAppDateType>
{
    /// <summary>
    /// Changes the CalendarAppDate relationships for a given calendar.
    /// </summary>
    /// <param name="calendarId">The ID of the calendar to update.</param>
    /// <param name="listCalendarAppDateAppDateType">The list of CalendarAppDateAppDateType entities to process.</param>
    /// <param name="autoSave">Indicates whether changes should be automatically saved to the database. Default is true.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A tuple containing two lists:
    /// <list type="bullet">
    /// <item><description>addAppDateOfCalendars: The list of CalendarAppDateAppDateType entities added to the calendar.</description></item>
    /// <item><description>removeAppDateOfCalendars: The list of CalendarAppDateAppDateType entities removed from the calendar.</description></item>
    /// </list>
    /// </returns>
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
