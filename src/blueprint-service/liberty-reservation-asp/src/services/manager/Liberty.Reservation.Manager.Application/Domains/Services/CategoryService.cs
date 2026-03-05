using Liberty.ApplicationShared.Utils;
using Liberty.Cache.Services;
using Liberty.Reservation.Manager.Application.Auth;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class CategoryService(
    ILogger<CategoryService> logger,
    ICacheService cacheService,
    IUnitOfWork unitOfWork,
    ISecurityContextAccessor securityContextAccessor,
    ICategoryRepository categoryRepository,
    IServiceProvider serviceProvider
) : BaseService<Category>(logger, cacheService, categoryRepository, new CategoryNotfoundException()), ICategoryService
{
    protected override string GetCacheKey(
        string methodName = "",
        params string[] keys
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;
        return $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}:" + base.GetCacheKey(methodName, keys);
    }

    protected override IQueryable<Category> GetQueryable()
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return base.GetQueryable()
            .Where(
                x => x.FacilityCategories!.Any(
                        y => y.FacilityId == facilityId
                    )
                    && !x.IsMaster
            );
    }

    public async Task<Category> CreateAsync(
        Category entityToCreate,
        CancellationToken cancellationToken = default
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var facilityCategoryRepository = serviceProvider.GetRequiredService<IFacilityCategoryRepository>();

        entityToCreate.Code = EntityUtil.CreateCode();
        entityToCreate.RecordMemo = EntityUtil.CreateRecordMemo();
        entityToCreate.IsMaster = false;

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var newCategory = await base.CreateAsync(
                entityToCreate,
                false,
                cancellationToken
            );

            var newFacilityCategory = new FacilityCategory
            {
                FacilityId = facilityId,
                Category = newCategory
            };
            await facilityCategoryRepository.AddAsync(
                newFacilityCategory,
                false,
                cancellationToken
            );

            await unitOfWork.CommitAsync(cancellationToken);

            return newCategory;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Create category failed: {Message}", ex.Message);
            await unitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
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
                existingEntity.Name?.UpdateLocalized(updateEntity.Name);
                existingEntity.Description ??= [];
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

    public override async Task<Category> DeleteAsync(
        long id,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        autoSave = false;

        var facilityId = securityContextAccessor.FacilityKey;

        var facilityCategoryRepository = serviceProvider.GetRequiredService<IFacilityCategoryRepository>();
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

            await facilityCategoryRepository.DeleteAsync(
                new FacilityCategory
                {
                    FacilityId = facilityId,
                    CategoryId = id
                },
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

        var queryable = GetQueryable().Where(x => x.CategoryType == type);

        if (pageable.IsEnabled is not null)
        {
            queryable = queryable.Where(x => x.IsEnabled == pageable.IsEnabled);
        }

        page = await queryable
            .OrderByDescending(x => x.DisplayOrder)
            .UsePageableAsync(pageable, cancellationToken: cancellationToken);

        await CacheService.SetAsync(
            cacheKey,
            page,
            cancellationToken
        );

        return page;
    }

    public async Task<int> CountByIdsAsync(
        long[] ids,
        CategoryTypes[] types,
        CancellationToken cancellationToken = default
    )
    {
        ids = [.. ids.OrderBy(x => x)];
        types = [.. types.OrderBy(x => x)];

        var cacheKey = GetCacheKey(
            nameof(FindAllByTypeAsync),
            $"{string.Join("_", ids)}_{string.Join("_", types)}"
        );
        var existingCount = await CacheService!.GetAsync<int?>(
            cacheKey,
            cancellationToken
        );

        if (existingCount is not null)
        {
            return existingCount.Value;
        }

        var queryable = GetQueryable().Where(x => ids.Contains(x.Id));

        if (types is { Length: > 0 })
        {
            queryable = queryable.Where(
                x => types.Contains(x.CategoryType)
            );
        }

        existingCount = await queryable.CountAsync(cancellationToken);

        await CacheService.SetAsync(
            cacheKey,
            existingCount,
            cancellationToken
        );

        return existingCount.Value;
    }

    public async Task<int> CountByIdsWithoutFacilityAsync(
        long[] ids,
        CategoryTypes[] types,
        bool useCache = true,
        CancellationToken cancellationToken = default
    )
    {
        ids = [.. ids.OrderBy(x => x)];
        types = [.. types.OrderBy(x => x)];

        var cacheKey = GetCacheKey(
            nameof(CountByIdsWithoutFacilityAsync),
            $"{string.Join("_", ids)}_{string.Join("_", types)}"
        );
        var existingCount = useCache
            ? await CacheService!.GetAsync<int?>(
                cacheKey,
                cancellationToken
            )
            : null;

        if (existingCount is not null)
        {
            return existingCount.Value;
        }

        var queryable = base.GetQueryable().Where(x => ids.Contains(x.Id));

        if (types is { Length: > 0 })
        {
            queryable = queryable.Where(
                x => types.Contains(x.CategoryType)
            );
        }

        existingCount = await queryable.CountAsync(cancellationToken);

        if (useCache)
        {
            await CacheService!.SetAsync(
                cacheKey,
                existingCount,
                cancellationToken
            );
        }

        return existingCount.Value;
    }

    public async Task<int> CountMasterByIdsAsync(
        long[] ids,
        CategoryTypes[] types,
        bool useCache = true,
        CancellationToken cancellationToken = default
    )
    {
        ids = [.. ids.OrderBy(x => x)];
        types = [.. types.OrderBy(x => x)];

        var cacheKey = GetCacheKey(
            nameof(CountMasterByIdsAsync),
            $"{string.Join("_", ids)}_{string.Join("_", types)}"
        );
        var existingCount = useCache
            ? await CacheService!.GetAsync<int?>(
                cacheKey,
                cancellationToken
            )
            : null;

        if (existingCount is not null)
        {
            return existingCount.Value;
        }

        var queryable = base.GetQueryable()
            .Where(x => x.IsMaster)
            .Where(x => ids.Contains(x.Id));

        if (types is { Length: > 0 })
        {
            queryable = queryable.Where(
                x => types.Contains(x.CategoryType)
            );
        }

        existingCount = await queryable.CountAsync(cancellationToken);

        if (useCache)
        {
            await CacheService!.SetAsync(
                cacheKey,
                existingCount,
                cancellationToken
            );
        }

        return existingCount.Value;
    }
}
