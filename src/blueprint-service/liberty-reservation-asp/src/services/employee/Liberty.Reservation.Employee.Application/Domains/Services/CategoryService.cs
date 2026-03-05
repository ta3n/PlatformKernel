using Liberty.ApplicationShared.Utils;
using Liberty.Cache.Services;
using Liberty.Pagination;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class CategoryService(
    ILogger<CategoryService> logger,
    IUnitOfWork unitOfWork,
    ICacheService cacheService,
    ICategoryRepository categoryRepository,
    IServiceProvider serviceProvider
) : BaseService<Category>(logger, cacheService, categoryRepository, new CategoryNotfoundException()), ICategoryService
{
    protected override IQueryable<Category> GetQueryable()
    {
        return categoryRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.IsMaster
            );
    }

    public override Task<Category> CreateAsync(
        Category entityToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        entityToCreate.Code = EntityUtil.CreateCode();
        entityToCreate.RecordMemo = EntityUtil.CreateRecordMemo();
        entityToCreate.IsMaster = true;

        return base.CreateAsync(entityToCreate, autoSave, cancellationToken);
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
                existingEntity.Name ??= [];
                existingEntity.Name.UpdateLocalized(updateEntity.Name);
                existingEntity.Description ??= [];
                existingEntity.Description.UpdateLocalized(updateEntity.Description);

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public override async Task<Category> DeleteAsync(
        long id,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        autoSave = false;

        var planCategoryRepository = serviceProvider.GetRequiredService<IPlanCategoryRepository>();
        var roomGroupCategoryRepository = serviceProvider.GetRequiredService<IRoomGroupCategoryRepository>();
        var optionItemCategoryRepository = serviceProvider.GetRequiredService<IOptionItemCategoryRepository>();
        var fileCategoryRepository = serviceProvider.GetRequiredService<IFileCategoryRepository>();

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var categoryDeleted = await base.DeleteAsync(
                id,
                autoSave,
                cancellationToken
            );

            var existingCategoriesOfPlan = await planCategoryRepository
                .GetQueryableWithAsNoTracking()
                .Where(x => x.CategoryId == id)
                .ToListAsync(cancellationToken);
            await planCategoryRepository.DeleteRangeAsync(
                existingCategoriesOfPlan,
                autoSave,
                cancellationToken
            );

            var existingCategoriesOfRoomGroup = await roomGroupCategoryRepository
                .GetQueryableWithAsNoTracking()
                .Where(x => x.CategoryId == id)
                .ToListAsync(cancellationToken);
            await roomGroupCategoryRepository.DeleteRangeAsync(
                existingCategoriesOfRoomGroup,
                autoSave,
                cancellationToken
            );

            var existingCategoriesOfOptionItem = await optionItemCategoryRepository
                .GetQueryableWithAsNoTracking()
                .Where(x => x.CategoryId == id)
                .ToListAsync(cancellationToken);
            await optionItemCategoryRepository.DeleteRangeAsync(
                existingCategoriesOfOptionItem,
                autoSave,
                cancellationToken
            );

            var existingCategoriesOfFile = await fileCategoryRepository
                .GetQueryableWithAsNoTracking()
                .Where(x => x.CategoryId == id)
                .ToListAsync(cancellationToken);
            await fileCategoryRepository.DeleteRangeAsync(
                existingCategoriesOfFile,
                autoSave,
                cancellationToken
            );

            await unitOfWork.CommitAsync(cancellationToken);

            return categoryDeleted;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Delete category failed: {Message}", ex.Message);
            await unitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    public async Task<IPage<Category>> FindAllByTypeAsync(
        IPageable pageable,
        CategoryTypes type,
        CancellationToken cancellationToken = default
    )
    {
        var pageableJson = JsonConvert.SerializeObject(
            pageable,
            JsonSettings.Optimized
        );

        var cacheKey = GetCacheKey(
            nameof(FindAllByTypeAsync),
            type.ToString(),
            pageableJson
        );
        IPage<Category>? page = await CacheService!.GetAsync<Page<Category>>(
            cacheKey,
            cancellationToken
        );

        if (page is not null)
        {
            return page;
        }

        page = await GetQueryable()
            .Where(x => x.CategoryType == type)
            .OrderByDescending(x => x.DisplayOrder)
            .UsePageableAsync(pageable, cancellationToken: cancellationToken);

        await CacheService.SetAsync(
            cacheKey,
            page,
            cancellationToken
        );

        return page;
    }

    public async Task<Category> FindByIdAsync(
        long id,
        CategoryTypes type,
        CancellationToken cancellationToken = default
    )
    {
        var cacheKey = GetCacheKey(
            nameof(FindByIdAsync),
            $"{type}_{id}"
        );

        var category = await CacheService!.GetAsync<Category>(
            cacheKey,
            cancellationToken
        );

        if (category is null)
        {
            category = await FindByIdAsync(id, cancellationToken);

            await CacheService.SetAsync(
                cacheKey,
                category,
                cancellationToken
            );
        }

        if (category.CategoryType != type)
        {
            throw new CategoryNotfoundException();
        }

        return category;
    }
}
