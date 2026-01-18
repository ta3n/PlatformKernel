using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services;

public interface ICheckFacilityService
{
    Task<bool> CheckUserManagedFacilityAsync(
        string facilityCode,
        string userCode,
        CancellationToken cancellationToken = default
    );

    Task<bool> CheckUserManagedFacilityAsync(
        string[] facilityCodes,
        string userCode,
        CancellationToken cancellationToken = default
    );

    Task<ManagerFacilityDtoResponse?> GetAllFacilitiesFromMembershipAsync(
        string userCode,
        CancellationToken cancellationToken = default
    );
}
