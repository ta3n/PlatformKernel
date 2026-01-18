using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationGetAllQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IReservationRepository reservationRepository
) : QueryPageBaseHandler<ReservationGetAllQuery, BookingReservationResponse>(mapper, cacheService)
{
    private readonly ReservationStatus[] _allowedReservationStates =
    [
        ReservationStatus.Confirmed,
        ReservationStatus.Reserved,
        ReservationStatus.UserCanceled,
        ReservationStatus.GuestCanceled,
        ReservationStatus.ManagerCanceled,
        ReservationStatus.Modified
    ];

    protected override string GetCacheKey(
        ReservationGetAllQuery request
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return CacheHelper.GetCacheKeyByEntity(
            nameof(Cancellation),
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
        ReservationGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = reservationRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.Facility)
            .Include(x => x.Reserver)
            .Where(x => x.Facility!.Id == facilityId)
            .Where(x => x.CheckInDate >= request.StartAppDateId)
            .Where(x => x.CheckInDate <= request.EndAppDateId)
            .Where(x => _allowedReservationStates.Contains(x.ReservationState))
            .OrderByDescending(x => x.ReservationDateTime)
            .ProjectTo<BookingReservationResponse>(Mapper.ConfigurationProvider);

        var page = await queryable.UsePageableAsync(
            request.Pageable,
            false,
            cancellationToken
        );

        var headers = page.GeneratePaginationHttpHeaders();
        var data = page.Content;

        return (headers, data);
    }
}
