using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.OptionItem;

public class OptionItemGetQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IOptionItemRepository optionItemRepository
) : QuerySingleBaseHandler<OptionItemGetQuery, OptionItemDetailResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        OptionItemGetQuery request
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;
        return CacheHelper.GetCacheKeyByEntity(
            nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.OptionItem),
            $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}",
            CacheHelper.ComputeHash(
                [
                    nameof(OptionItemGetQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, OptionItemDetailResponse)> HandleAsync(
        OptionItemGetQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = optionItemRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.OptionItemCategories!)
            .ThenInclude(x => x.Category)
            .Include(x => x.OptionItemQuestions!)
            .ThenInclude(x => x.Question)
            .Include(x => x.FileOptionItems!)
            .ThenInclude(t => t.File)
            .Include(x => x.FacilityOptionItems)
            .Where(x => x.Id == request.Id)
            .Where(
                x => x.FacilityOptionItems!.Any(
                    t => t.FacilityId == facilityId
                )
            )
            .ProjectTo<OptionItemDetailResponse>(Mapper.ConfigurationProvider);

        var data = await queryable.SingleOrDefaultAsync(
                cancellationToken
            )
            ?? throw new OptionItemNotfoundException();

        return (new HeaderDictionary(), data);
    }
}
