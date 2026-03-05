using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class MealTypeService(
    ILogger<MealTypeService> logger,
    IMealTypeRepository mealTypeRepository
) : BaseService<MealType>(logger, mealTypeRepository, new MealTypeNotfoundException()), IMealTypeService
{
    public override async Task<IPage<MealType>> FindAllAsync(
        IPageable pageable,
        CancellationToken cancellationToken = default
    )
    {
        var key = JsonConvert.SerializeObject(
            pageable,
            JsonSettings.Optimized
        );

        var cacheKey = GetCacheKey(
            nameof(FindAllAsync),
            key
        );
        IPage<MealType>? page = null;
        if (CacheService is not null)
        {
            page = await CacheService.GetAsync<Page<MealType>>(
                cacheKey,
                cancellationToken
            );
        }

        if (page is null)
        {
            page = await GetQueryable()
                .OrderBy(x => x.DisplayOrder)
                .UsePageableAsync(
                    pageable,
                    cancellationToken: cancellationToken
                );

            if (CacheService is not null)
            {
                await CacheService.SetAsync(
                    cacheKey,
                    page,
                    cancellationToken
                );
            }
        }

        logger.LogInformation(
            "{BaseServiceName}.{FindAllAsyncName} - Successfully found all {EntityDataName} {Count}",
            nameof(MealType),
            nameof(FindAllAsync),
            nameof(MealType),
            page.Content.Count()
        );

        return page;
    }
}
