using Liberty.Pagination;
using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface ICategoryService : IBaseService<Category>
{
    Task<IPage<Category>> FindAllByTypeAsync(
        IPageable pageable,
        CategoryTypes type,
        CancellationToken cancellationToken = default
    );

    Task<Category> FindByIdAsync(
        long id,
        CategoryTypes type,
        CancellationToken cancellationToken = default
    );
}
