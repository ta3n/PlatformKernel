using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class FacilityCategoryService(
    ILogger<FacilityCategoryService> logger,
    IFacilityCategoryRepository facilityCategoryRepository
) : BaseServiceRelation<FacilityCategory>(logger, facilityCategoryRepository), IFacilityCategoryService
{
    public async Task<IEnumerable<FacilityCategory>> FindAllFacilitiesOfCategoryAsync(
        long categoryId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = facilityCategoryRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.CategoryId == categoryId);

        var facilitiesOfCategory = await queryable.ToListAsync(cancellationToken);

        return facilitiesOfCategory;
    }
}
