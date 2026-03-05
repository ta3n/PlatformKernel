using Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Dtos;
using Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Services;

namespace Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;

public class MockCheckFacilityService : ICheckFacilityService
{
    public Task<ManagerFacilityDto?> CheckUserManagedFacilityAsync(
        string facilityCode,
        string managerId,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult<ManagerFacilityDto?>(
            new ManagerFacilityDto
            {
                Code = facilityCode,
                Name = "Facility 1",
                IsEnabled = true
            }
        );
    }

    public Task<ManagerFacilityDtoResponse?> GetAllFacilitiesFromMembershipAsync(
        string managerId,
        CancellationToken cancellationToken = default
    )
    {
        var data = new ManagerFacilityDtoResponse
        {
            Facilities =
            [
                new()
                {
                    Code = "FAC001",
                    Name = "Facility 1",
                    IsEnabled = true
                },

                new()
                {
                    Code = "FAC002",
                    Name = "Facility 2",
                    IsEnabled = true
                }
            ]
        };

        return Task.FromResult(data)!;
    }
}
