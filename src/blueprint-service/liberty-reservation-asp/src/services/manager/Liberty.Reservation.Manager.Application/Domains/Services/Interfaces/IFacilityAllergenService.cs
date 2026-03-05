using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IFacilityAllergenService : IBaseServiceRelation<FacilityAllergen>
{
    Task<(IEnumerable<FacilityAllergen> adds, IEnumerable<FacilityAllergen> removes)> ChangeFacilityAllergenAsync(
        long facilityId,
        IEnumerable<long> facilityAllergenIds,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );
}
