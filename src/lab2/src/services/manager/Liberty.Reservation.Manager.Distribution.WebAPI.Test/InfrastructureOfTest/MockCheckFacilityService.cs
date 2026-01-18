using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Test.InfrastructureOfTest;

public class MockCheckFacilityService : ICheckFacilityService
{
    public Task<bool> CheckUserManagedFacilityAsync(
        string facilityCode,
        string userCode,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(true);
    }

    public Task<bool> CheckUserManagedFacilityAsync(
        string[] facilityCodes,
        string userCode,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(true);
    }

    public Task<ManagerFacilityDtoResponse?> GetAllFacilitiesFromMembershipAsync(
        string userCode,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult<ManagerFacilityDtoResponse?>(new ManagerFacilityDtoResponse { Facilities = [] });
    }
}
