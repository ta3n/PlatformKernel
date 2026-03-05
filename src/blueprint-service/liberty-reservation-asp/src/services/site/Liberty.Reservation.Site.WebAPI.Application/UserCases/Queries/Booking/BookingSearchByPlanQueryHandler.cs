using Newtonsoft.Json;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;

public class BookingSearchByPlanQueryHandler(
    IMapper mapper,
    IMediator mediator,
    ISecurityContextAccessor securityContextAccessor,
    ICacheService cacheService
) : QueryPageBaseHandler<BookingSearchByPlanQuery, BookingSearchByPlanResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        BookingSearchByPlanQuery request
    )
    {
        var facilityId = securityContextAccessor.GetFacilityIdSelected();
        var siteId = securityContextAccessor.GetSiteIdSelected();

        return CacheHelper.GetCacheKeyByParameters(
            string.Format(
                CacheKeys.BookingSearchPrefixKey,
                facilityId,
                siteId
            ),
            CacheHelper.ComputeHash(
                [
                    nameof(BookingSearchByPlanQuery),
                    JsonConvert.SerializeObject(request.Payload, JsonSettings.Optimized)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, IEnumerable<BookingSearchByPlanResponse>)> HandleAsync(
        BookingSearchByPlanQuery request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var siteId = securityContextAccessor.GetSiteIdSelected();
        var facilityId = securityContextAccessor.GetFacilityIdSelected();

        var (headers, bookingSearchResponse) = await mediator.Send(
            new Reservation.Application.UseCases.Queries.BookingReservation.BookingSearchByPlanQuery(
                payload,
                facilityId,
                siteId,
                request.Pageable
            ) { IsLoadingPriceAppDate = true },
            cancellationToken
        );

        return (headers, bookingSearchResponse);
    }
}
