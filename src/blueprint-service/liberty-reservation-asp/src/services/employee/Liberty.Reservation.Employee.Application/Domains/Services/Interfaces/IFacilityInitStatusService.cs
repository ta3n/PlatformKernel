using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Models;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface IFacilityInitStatusService : IBaseService<Facility>
{
    Task<FacilitySeedDataStatus> GetFacilitySeedDataStatusAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    );
}
