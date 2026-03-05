using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IFacilityCategoryService : IBaseServiceRelation<FacilityCategory>
{
    Task<(IEnumerable<FacilityCategory> adds, IEnumerable<FacilityCategory> removes)> ChangeFacilityCategoryAsync(
        long facilityId,
        List<long> facilityCategoryIds,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );
}
