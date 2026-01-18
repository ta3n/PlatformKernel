using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Specifications;
using NodaTime;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationGetAllByFilterQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IReservationRepository reservationRepository,
    IBookingReservationService bookingReservationService
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
        var timeZone = bookingReservationService.GetFacilityTimeZoneById(
            facilityId
        );

        var spec = new ReservationGetAllByFilterQuerySpec(
            query.Request,
            timeZone,
            query.Pageable
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
