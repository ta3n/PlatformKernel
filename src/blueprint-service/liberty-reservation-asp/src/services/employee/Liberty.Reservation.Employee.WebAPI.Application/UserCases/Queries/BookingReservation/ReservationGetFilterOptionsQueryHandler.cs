using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationGetFilterOptionsQueryHandler(
    IMapper mapper,
    ICacheService cacheService
) : QuerySingleBaseHandler<ReservationGetFilterOptionsQuery, ReservationFilterOptionsResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        ReservationGetFilterOptionsQuery request
    )
    {
        return CacheHelper.GetCacheKeyByParameters(
            nameof(ReservationGetAllByFilterQuery),
            CacheHelper.ComputeHash(
                [
                    nameof(ReservationGetAllByFilterQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override Task<(IHeaderDictionary, ReservationFilterOptionsResponse)> HandleAsync(
        ReservationGetFilterOptionsQuery request,
        CancellationToken cancellationToken
    )
    {
        var response = new ReservationFilterOptionsResponse(
            [.. ReservationStatusFilter.AllowedStatuses],
            [.. Enum.GetValues<PaymentTypes>().Where(x => x != PaymentTypes.Unknown)]
        );

        return Task.FromResult<(IHeaderDictionary, ReservationFilterOptionsResponse)>((new HeaderDictionary(), response));
    }
}
