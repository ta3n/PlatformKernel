using AutoMapper.QueryableExtensions;
using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.PersonAgeType;

public class PersonAgeTypeGetAllQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    IPersonAgeTypeRepository personAgeTypeRepository
) : QueryPageBaseHandler<PersonAgeTypeGetAllQuery, PersonAgeTypeResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        PersonAgeTypeGetAllQuery request
    )
    {
        return CacheHelper.GetCacheKeyByParameters(
            CacheKeys.PersonAgeTypeMasterGetAllPrefixKey,
            CacheHelper.ComputeHash(
                [
                    nameof(PersonAgeTypeGetAllQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, IEnumerable<PersonAgeTypeResponse>)> HandleAsync(
        PersonAgeTypeGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var queryable = personAgeTypeRepository.GetQueryableWithAsNoTracking()
            .Where(x => x.IsMaster)
            .OrderByDescending(x => x.IsMain)
            .ThenBy(x => x.Id);

        var selectQuery = queryable.ProjectTo<PersonAgeTypeResponse>(Mapper.ConfigurationProvider);

        var page = await selectQuery.UsePageableAsync(
            request.Pageable,
            cancellationToken: cancellationToken
        );

        var header = page.GeneratePaginationHttpHeaders();

        var data = page.Content;

        return (header, data);
    }
}
