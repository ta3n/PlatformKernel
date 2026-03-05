using Liberty.Pagination;

namespace Liberty.Reservation.Site.Application.Domains.Services.Interfaces;

public interface ICategoryService : IBaseService<Category>
{
    Task<IPage<Category>> FindAllByTypeAsync(
        IPageable pageable,
        CategoryTypes type
    );

    Task<Category> FindByIdAsync(
        long id,
        CategoryTypes type
    );
}
