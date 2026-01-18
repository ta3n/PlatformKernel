namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;

public class BookingGetFacilityQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IFacilityRepository facilityRepository,
    ISiteRepository siteRepository,
    IFacilityRoomGroupRepository facilityRoomGroupRepository
) : QuerySingleBaseHandler<BookingGetFacilityQuery, BookingFacilityResponse>(mapper, cacheService)
{
    private const int DefaultMaxPeople = 5;

    protected override string GetCacheKey(
        BookingGetFacilityQuery request
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
                    nameof(BookingGetFacilityQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, BookingFacilityResponse)> HandleAsync(
        BookingGetFacilityQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.GetFacilityIdSelected();
        var siteId = securityContextAccessor.GetSiteIdSelected();

        var facility = await GetFacilityAsync(
            facilityId,
            cancellationToken
        );
        var site = await GetSiteOfBookingFacilityAsync(
            siteId,
            facilityId,
            cancellationToken
        );
        var maxPeople = await GetMaxPeopleAsync(
            facilityId,
            cancellationToken
        );

        site.MaxPeople = maxPeople;
        facility.Site = site;

        return (new HeaderDictionary(), facility);
    }

    private async Task<BookingFacilityResponse> GetFacilityAsync(
        long facilityId,
        CancellationToken cancellationToken
    )
    {
        var queryable = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == facilityId && x.IsEnabled)
            .ProjectTo<BookingFacilityResponse>(Mapper.ConfigurationProvider)
            .AsSingleQuery();

        var facility = await queryable.FirstOrDefaultAsync(cancellationToken)
            ?? throw new FacilityNotfoundException();

        return facility;
    }

    private async Task<SiteOfBookingFacilityResponse> GetSiteOfBookingFacilityAsync(
        long siteId,
        long facilityId,
        CancellationToken cancellationToken
    )
    {
        var queryable = siteRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == siteId && x.IsEnabled)
            .Where(x => x.FacilitySites!.Any(y => y.FacilityId == facilityId))
            .Select(
                x => new SiteOfBookingFacilityResponse(
                    x.Code,
                    x.Name!.GetValueByHeader(DefaultValues.LanguageCode)
                )
            )
            .AsSingleQuery();

        var site = await queryable.FirstOrDefaultAsync(cancellationToken)
            ?? throw new SiteNotAlreadyInFacilityException();

        return site;
    }

    private async Task<int> GetMaxPeopleAsync(
        long facilityId,
        CancellationToken cancellationToken
    )
    {
        var queryable = facilityRoomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.FacilityId == facilityId)
            .Where(x => x.RoomGroup!.IsEnabled)
            .Select(x => x.RoomGroup!.CapacityMax);

        var maxPeople = await queryable.MaxAsync(cancellationToken);

        return maxPeople ?? DefaultMaxPeople;
    }
}
