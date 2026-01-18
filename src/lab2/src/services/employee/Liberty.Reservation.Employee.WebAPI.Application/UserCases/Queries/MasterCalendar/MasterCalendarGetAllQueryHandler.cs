using AutoMapper.QueryableExtensions;
using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.MasterCalendar;

public class MasterCalendarGetAllQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    IAppDateRepository appDateRepository
) : QueryPageBaseHandler<MasterCalendarGetAllQuery, DateOfMasterCalendarResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        MasterCalendarGetAllQuery request
    )
    {
        return CacheHelper.GetCacheKeyByEntity(
            nameof(AppDate),
            CacheHelper.ComputeHash(
                [
                    nameof(MasterCalendarGetAllQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, IEnumerable<DateOfMasterCalendarResponse>)> HandleAsync(
        MasterCalendarGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var queryable = appDateRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.AppDateAppDateTypes!)
            .ThenInclude(x => x.AppDateType)
            .Include(x => x.AppDateAppDateDatas!)
            .ThenInclude(x => x.AppDateData)
            .Where(x => x.Id >= request.StartDate && x.Id <= request.EndDate)
            .Where(
                x =>
                    x.AppDateAppDateDatas!.Count > 0
                    || (x.AppDateAppDateTypes!.Count > 0 && x.AppDateAppDateTypes!.Any(y => y.AppDateType!.IsEnabled))
            )
            .OrderBy(x => x.Id)
            .ProjectTo<DateOfMasterCalendarResponse>(Mapper.ConfigurationProvider);

        var page = await queryable.UsePageableAsync(
            request.Pageable,
            cancellationToken: cancellationToken
        );

        var data = page.Content;
        var headers = page.GeneratePaginationHttpHeaders();

        return (headers, data);
    }
}
