using Liberty.ApplicationShared.Utils;
using Liberty.Pagination;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Site.Application.Exceptions;

namespace Liberty.Reservation.Site.Application.Domains.Services;

public class CategoryService(
    ILogger<CategoryService> logger,
    ICategoryRepository categoryRepository
) : BaseService<Category>(logger, categoryRepository, new CategoryNotfoundException()), ICategoryService
{
    public override Task<Category> CreateAsync(
        Category entityToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        entityToCreate.Code = EntityUtil.CreateCode();
        entityToCreate.RecordMemo = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        return base.CreateAsync(
            entityToCreate,
            autoSave,
            cancellationToken
        );
    }

    public override Task<Category> UpdateAsync(
        Category entityToUpdate,
        bool autoSave = true,
        Func<Category, Category, Category>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.Name?.UpdateLocalized(updateEntity.Name);
                existingEntity.Description?.UpdateLocalized(updateEntity.Description);

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public async Task<IPage<Category>> FindAllByTypeAsync(
        IPageable pageable,
        CategoryTypes type
    )
    {
        var page = await categoryRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.CategoryType == type)
            .UsePageableAsync(pageable);

        return page;
    }

    public async Task<Category> FindByIdAsync(
        long id,
        CategoryTypes type
    )
    {
        var category = await FindByIdAsync(id);
        if (category.CategoryType != type)
        {
            throw new CategoryNotfoundException();
        }

        return category;
    }
}
