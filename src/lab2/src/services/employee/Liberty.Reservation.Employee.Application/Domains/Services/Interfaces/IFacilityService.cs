using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface IFacilityService : IBaseService<Facility>
{
    Task<Facility> UpdateFaxOfFacilityAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<Facility>> FindAllFacilitiesHasCategoryAsync(
        long categoryId,
        CancellationToken cancellationToken = default
    );
}
