using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Specifications;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationGetAllByFilterQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IReservationRepository reservationRepository
) : QueryPageBaseHandler<ReservationGetAllByFilterQuery, BookingReservationResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        ReservationGetAllByFilterQuery request
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return CacheHelper.GetCacheKeyByEntity(
            nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation),
            $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}",
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
        var facilityId = securityContextAccessor.FacilityKey;

        var spec = new ReservationGetAllByFilterQuerySpec(
            query.Request
        );

        var queryable = reservationRepository
            .GetQueryableWithAsNoTracking(spec)
            .IgnoreQueryFilters()
            .Where(x => x.FacilityId == facilityId)
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
