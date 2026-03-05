using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IFacilityPersonAgeTypeService : IBaseServiceRelation<FacilityPersonAgeType>
{
    Task<List<FacilityPersonAgeType>> FindAllByFacilityIdAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    );

    Task<FacilityPersonAgeType?> FindByFacilityIdAsync(
        long facilityId,
        long personAgeTypeId,
        CancellationToken cancellationToken = default
    );
}
