using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class CalendarService(
    ILogger<CalendarService> logger,
    ISecurityContextAccessor securityContextAccessor,
    ICalendarRepository calendarRepository
) : BaseService<Calendar>(logger, calendarRepository, new CalendarNotFoundException()), ICalendarService
{
    protected override IQueryable<Calendar> GetQueryable()
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return base.GetQueryable()
            .Where(
                x => x.FacilityCalendars!.Any(
                    t => t.FacilityId == facilityId
                )
            );
    }
}
