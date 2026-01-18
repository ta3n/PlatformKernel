using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface IFacilityInitDefaultDataService : IBaseService<Facility>
{
    Task<IEnumerable<FacilityPersonAgeType>> AddDefaultPersonAgeTypesAsync(
        Facility facility,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );
}
