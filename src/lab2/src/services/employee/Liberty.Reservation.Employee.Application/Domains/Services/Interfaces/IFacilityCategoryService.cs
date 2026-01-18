using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface IFacilityCategoryService : IBaseServiceRelation<FacilityCategory>
{
    Task<IEnumerable<FacilityCategory>> FindAllFacilitiesOfCategoryAsync(
        long categoryId,
        CancellationToken cancellationToken = default
    );
}
