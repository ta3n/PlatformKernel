using AutoMapper.QueryableExtensions;
using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Facility;

public class FacilityGetAllDestinationsQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    IFacilitySiteRepository facilitySiteRepository
) : QueryPageBaseHandler<FacilityGetAllDestinationsQuery, DestinationResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        FacilityGetAllDestinationsQuery request
    )
    {
        return CacheHelper.GetCacheKeyByEntity(
            nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.Facility),
            CacheHelper.ComputeHash(
                [
                    nameof(FacilityGetAllDestinationsQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, IEnumerable<DestinationResponse>)> HandleAsync(
        FacilityGetAllDestinationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var queryable = facilitySiteRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.Site!)
            .Where(x => !x.IsDeleted && x.FacilityId == request.FacilityId);

        if (request.Pageable.IsEnabled is not null)
        {
            queryable = queryable.Where(x => x.Site!.IsEnabled == request.Pageable.IsEnabled);
        }

        queryable = queryable.OrderByDescending(x => x.Site!.DisplayOrder);

        var page = await queryable
            .ProjectTo<DestinationResponse>(Mapper.ConfigurationProvider)
            .UsePageableAsync(
                request.Pageable,
                cancellationToken: cancellationToken
            );

        var data = page.Content;
        var headers = page.GeneratePaginationHttpHeaders();

        var response = (headers, data);

        return response;
    }
}
