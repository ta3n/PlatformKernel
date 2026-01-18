using AutoMapper.QueryableExtensions;
using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Specifications;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationGetAllByFilterQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    IReservationRepository reservationRepository
) : QueryPageBaseHandler<ReservationGetAllByFilterQuery, BookingReservationResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        ReservationGetAllByFilterQuery request
    )
    {
        return CacheHelper.GetCacheKeyByEntity(
            nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation),
            CacheKeys.AllFacilityBookingSearchPrefixKey,
            CacheHelper.ComputeHash(
                [
                    nameof(ReservationGetAllByFilterQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, IEnumerable<BookingReservationResponse>)> HandleAsync(
        ReservationGetAllByFilterQuery query,
        CancellationToken cancellationToken
    )
    {
        var spec = new ReservationGetAllByFilterQuerySpec(
            query.Request
        );

        var queryable = reservationRepository
            .GetQueryableWithAsNoTracking(spec)
            .IgnoreQueryFilters()
            .ProjectTo<BookingReservationResponse>(Mapper.ConfigurationProvider);

        var page = await queryable.UsePageableAsync(
            query.Pageable,
            false,
            cancellationToken
        );

        var headers = page.GeneratePaginationHttpHeaders();
        var data = page.Content;

        return (headers, data);
    }
}
