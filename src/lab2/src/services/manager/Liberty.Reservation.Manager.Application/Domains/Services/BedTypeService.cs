using Liberty.Cache.Services;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class BedTypeService(
    ILogger<BedTypeService> logger,
    ICacheService cacheService,
    IBedTypeRepository bedTypeRepository
) : BaseService<BedType>(logger, cacheService, bedTypeRepository, new BedTypeNotfoundException()),
    IBedTypeService
{
    public override async Task<IPage<BedType>> FindAllAsync(
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
        IPage<BedType>? page = null;
        if (CacheService is not null)
        {
            page = await CacheService.GetAsync<Page<BedType>>(
                cacheKey,
                cancellationToken
            );
        }

        if (page is not null)
        {
            return page;
        }

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

        return page;
    }
}
