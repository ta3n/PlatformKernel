using Newtonsoft.Json;
using System.Text;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;

public class BookingGetDetailsQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IBookingPlanRoomGroupSitePersonAgeTypeRepository personAgeTypeRepository,
    ICacheService cacheService,
    IPlanRepository planRepository,
    IRoomGroupRepository roomGroupRepository,
    ICheckChangedService checkChangedService
) : QuerySingleBaseHandler<BookingGetDetailsQuery, BookingDetailsResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        BookingGetDetailsQuery request
    )
    {
        var facilityId = securityContextAccessor.GetFacilityIdSelected();
        var siteId = securityContextAccessor.GetSiteIdSelected();

        return CacheHelper.GetCacheKeyByParameters(
            string.Format(
                CacheKeys.BookingDetailPrefixKey,
                facilityId,
                siteId,
                request.PlanId,
                request.RoomGroupId
            ),
            CacheHelper.ComputeHash(
                [
                    nameof(BookingGetDetailsQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, BookingDetailsResponse)> HandleAsync(
        BookingGetDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.GetFacilityIdSelected();
        var siteId = securityContextAccessor.GetSiteIdSelected();

        var bookingResponse = await GetBookingDetailAsync(
            facilityId,
            siteId,
            request,
            cancellationToken
        );

        var roomGroupResponse = await GetRoomGroupDetailAsync(
            siteId,
            request,
            cancellationToken
        );

        var personAgeTypeResponse = await GetAllPersonAgeTypesAsync(
            siteId,
            request,
            cancellationToken
        );

        var lastUpdateObject = await checkChangedService.GetLastUpdatedAtAsync(
            request.PlanId,
            request.RoomGroupId,
            cancellationToken
        );
        var lastUpdateSerialized = JsonConvert.SerializeObject(lastUpdateObject);
        var lastUpdateBytes = Encoding.UTF8.GetBytes(lastUpdateSerialized);
        var lastUpdateBase64 = Convert.ToBase64String(lastUpdateBytes);

        bookingResponse.PersonAgeTypes = [.. personAgeTypeResponse];
        bookingResponse.RoomGroup = roomGroupResponse;
        bookingResponse.LastUpdateString = lastUpdateBase64;

        return (new HeaderDictionary(), bookingResponse);
    }

    private async Task<BookingDetailsResponse> GetBookingDetailAsync(
        long facilityId,
        long siteId,
        BookingGetDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        var bookingSearchDate = DateTime.UtcNow.AddHours(
            DefaultValues.TimeZoneOffset
        );
        var bookingSearchDateId = AppDate.GetId(bookingSearchDate);

        var queryable = planRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.PlanRoomGroupSites!.Any(
                    y => y.SiteId == siteId
                        && y.RoomGroupId == request.RoomGroupId
                        && x.IsEnabled
                )
            )
            .Where(
                x => x.Id == request.PlanId
                    && x.IsEnabled
                    && x.FacilityPlans!
                        .Any(y => y.IsEnabled && y.FacilityId == facilityId)
            )
            .Where(
                x => x.PlanRoomGroups!
                    .Any(
                        y => y.RoomGroupId == request.RoomGroupId
                            && y.RoomGroup!.IsEnabled
                    )
            )
            .Where(x => !x.UseDisplayDate || x.DisplayDateStart == null || x.DisplayDateStart <= bookingSearchDateId)
            .Where(x => !x.UseDisplayDate || x.DisplayDateEnd == null || x.DisplayDateEnd >= bookingSearchDateId)
            .ProjectTo<BookingDetailsResponse>(Mapper.ConfigurationProvider)
            .AsSingleQuery();

        var bookingResponse = await queryable.FirstOrDefaultAsync(
                cancellationToken
            )
            ?? throw new BookingNotfoundException();

        return bookingResponse;
    }

    private async Task<RoomGroupResponse> GetRoomGroupDetailAsync(
        long siteId,
        BookingGetDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        var queryable = roomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.Id == request.RoomGroupId
                    && x.IsEnabled
            )
            .Where(
                x => x.PlanRoomGroups!.Any(
                    y => y.PlanId == request.PlanId
                )
            )
            .Where(
                x => x.PlanRoomGroupSites!.Any(
                    y => y.SiteId == siteId
                        && y.PlanId == request.PlanId
                        && x.IsEnabled
                )
            )
            .ProjectTo<RoomGroupResponse>(Mapper.ConfigurationProvider)
            .AsSingleQuery();

        var roomGroupResponse = await queryable.FirstOrDefaultAsync(
                cancellationToken
            )
            ?? throw new RoomGroupNotfoundException();

        return roomGroupResponse;
    }

    private async Task<IEnumerable<PersonAgeTypeOfBookingFacilityResponse>> GetAllPersonAgeTypesAsync(
        long siteId,
        BookingGetDetailsQuery request,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = personAgeTypeRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x =>
                    x.PlanId == request.PlanId
                    && x.SiteId == siteId
                    && x.RoomGroupId == request.RoomGroupId
                    && x.IsEnabled
                    && x.PersonAgeType!.IsEnabled
                    && x.PersonAgeType!.IsVisible
            )
            .OrderBy(x => x.PersonAgeType!.DisplayOrder)
            .Select(
                x =>
                    new PersonAgeTypeOfBookingFacilityResponse(
                        x.PersonAgeTypeId,
                        x.PersonAgeType!.IsMain,
                        x.PersonAgeType!.AgeMin,
                        x.PersonAgeType!.AgeMax,
                        x.PersonAgeType!.Name!.GetValueByHeader(DefaultValues.LanguageCode),
                        x.PersonAgeType!.UpdatedAt
                    )
            );

        return await queryable.ToListAsync(cancellationToken);
    }
}
