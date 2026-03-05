using AutoMapper;
using AutoMapper.QueryableExtensions;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Liberty.Reservation.Application.UseCases.Queries.AlertMessage;

public class GetAllAlertMessageActiveQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    IAlertMessageRepository alertMessageRepository
) : QueryListBaseHandler<GetAllAlertMessageActiveQuery, AlertMessageResponse>(mapper, cacheService)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<AlertMessageResponse>)> HandleAsync(
        GetAllAlertMessageActiveQuery request,
        CancellationToken cancellationToken
    )
    {
        var keyCache = $"*{CacheKeys.AllAlertMessageActivePrefixKey}.{request.Language}*";
        var alertMessagesCached = CacheService!
            .GetByPatterns<AlertMessageResponse>(
                [keyCache]
            );

        if (alertMessagesCached != null && alertMessagesCached.Any())
        {
            return (new HeaderDictionary(), alertMessagesCached);
        }

        var query = alertMessageRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.IsEnabled)
            .ProjectTo<AlertMessageResponse>(Mapper.ConfigurationProvider);

        var response = await query
            .ToListAsync(cancellationToken);

        await CacheService!.SetAsync<IEnumerable<AlertMessageResponse>>(
            keyCache,
            response,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60 * 24 * 365) },
            cancellationToken
        );

        return (new HeaderDictionary(), response);
    }
}
