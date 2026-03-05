namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class FacilityCalendarService(
    ILogger<FacilityCalendarService> logger,
    IFacilityCalendarRepository repository
) : BaseServiceRelation<FacilityCalendar>(logger, repository), IFacilityCalendarService
{
    public async Task<FacilityCalendar?> FindByFacilityIdAsync(
        long facilityId,
        CancellationToken cancellationToken
    )
    {
        return await repository.GetQueryableWithAsNoTracking()
            .Include(x => x.Calendar)
            .FirstOrDefaultAsync(x => x.FacilityId == facilityId, cancellationToken);
    }
}
