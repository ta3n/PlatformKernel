using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IFacilityCalendarService : IBaseServiceRelation<FacilityCalendar>
{
    Task<FacilityCalendar?> FindByFacilityIdAsync(
        long facilityId,
        CancellationToken cancellationToken
    );
}
