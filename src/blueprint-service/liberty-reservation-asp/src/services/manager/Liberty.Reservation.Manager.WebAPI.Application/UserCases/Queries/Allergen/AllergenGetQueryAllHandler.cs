using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Allergen;

public class AllergenGetQueryAllHandler(
    IMapper mapper,
    ICacheService cacheService,
    IAllergenRepository allergenRepository
) : QueryPageBaseHandler<AllergenGetAllQuery, AllergenResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        AllergenGetAllQuery request
    )
    {
        return CacheHelper.GetCacheKeyByEntity(
            nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.Allergen),
            CacheHelper.ComputeHash(
                [
                    nameof(AllergenGetQueryAllHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, IEnumerable<AllergenResponse>)> HandleAsync(
        AllergenGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var queryable = allergenRepository
            .GetQueryableWithAsNoTracking();

        var page = await queryable.UsePageableAsDtoAsync<
            Reservation.Application.Contexts.DataContexts.Entities.Data.Allergen,
            AllergenResponse>(
            request.Pageable,
            Mapper,
            cancellationToken: cancellationToken
        );

        var data = page.Content;
        var headers = page.GeneratePaginationHttpHeaders();

        return (headers, data);
    }
}
