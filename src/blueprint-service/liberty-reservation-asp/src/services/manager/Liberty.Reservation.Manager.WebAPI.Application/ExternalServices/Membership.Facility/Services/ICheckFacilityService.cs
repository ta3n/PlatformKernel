using Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Dtos;

namespace Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Services;

public interface ICheckFacilityService
{
    Task<ManagerFacilityDto?> CheckUserManagedFacilityAsync(
        string facilityCode,
        string managerId,
        CancellationToken cancellationToken = default
    );

    Task<ManagerFacilityDtoResponse?> GetAllFacilitiesFromMembershipAsync(
        string managerId,
        CancellationToken cancellationToken = default
    );
}
