using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface ICategoryService : IBaseService<Category>
{
    Task<Category> CreateAsync(
        Category entityToCreate,
        CancellationToken cancellationToken = default
    );

    Task<IPage<Category>> FindAllByTypeAsync(
        IPageable pageable,
        CategoryTypes type,
        CancellationToken cancellationToken = default
    );

    Task<int> CountByIdsAsync(
        long[] ids,
        CategoryTypes[] types,
        CancellationToken cancellationToken = default
    );

    Task<int> CountByIdsWithoutFacilityAsync(
        long[] ids,
        CategoryTypes[] types,
        bool useCache = true,
        CancellationToken cancellationToken = default
    );

    Task<int> CountMasterByIdsAsync(
        long[] ids,
        CategoryTypes[] types,
        bool useCache = true,
        CancellationToken cancellationToken = default
    );
}
