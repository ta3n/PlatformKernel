using Newtonsoft.Json;
using System.Text;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;

public class BookingGetPlanByCodeQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IBookingPlanRoomGroupSitePersonAgeTypeRepository personAgeTypeRepository,
    ICacheService cacheService,
    IPlanRepository planRepository,
    IRoomGroupRepository roomGroupRepository,
    ICheckChangedService checkChangedService
) : QuerySingleBaseHandler<BookingGetPlanByCodeQuery, BookingDetailsResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        BookingGetPlanByCodeQuery request
    )
    {
        var facilityId = securityContextAccessor.GetFacilityIdSelected();
        var siteId = securityContextAccessor.GetSiteIdSelected();

        return CacheHelper.GetCacheKeyByParameters(
            string.Format(
                CacheKeys.BookingDetailPrefixKey,
                facilityId,
                siteId,
                request.PlanCode,
                request.RoomGroupCode
            ),
            CacheHelper.ComputeHash(
                [
                    nameof(BookingGetPlanByCodeQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, BookingDetailsResponse)> HandleAsync(
        BookingGetPlanByCodeQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.GetFacilityIdSelected();
        var siteId = securityContextAccessor.GetSiteIdSelected();

        var planId = await planRepository
                           .GetQueryableWithAsNoTracking()
                           .Where(x => x.Code == request.PlanCode && x.IsEnabled)
                           .Select(x => x.Id)
                           .SingleAsync();

        var roomGroupId = await roomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Code == request.RoomGroupCode && x.IsEnabled)
            .Select(x => x.Id)
            .SingleAsync();

        var bookingResponse = await GetBookingDetailAsync(
            facilityId,
            siteId,
            planId,
            roomGroupId,
            cancellationToken
        );

        var roomGroupResponse = await GetRoomGroupDetailAsync(
            siteId,
            planId,
            roomGroupId,
            cancellationToken
        );

        var personAgeTypeResponse = await GetAllPersonAgeTypesAsync(
            siteId,
            planId,
            roomGroupId,
            cancellationToken
        );

        var lastUpdateObject = await checkChangedService.GetLastUpdatedAtAsync(
            planId,
            roomGroupId,
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
        long planId,
        long roomGroupId,
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
                        && y.RoomGroupId == roomGroupId
                        && x.IsEnabled
                )
            )
            .Where(
                x => x.Id ==  planId
                    && x.IsEnabled
                    && x.FacilityPlans!
                        .Any(y => y.IsEnabled && y.FacilityId == facilityId)
            )
            .Where(
                x => x.PlanRoomGroups!
                    .Any(
                        y => y.RoomGroupId == roomGroupId
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
        long planId,
        long roomGroupId,
        CancellationToken cancellationToken
    )
    {
        var queryable = roomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.Id == roomGroupId
                    && x.IsEnabled
            )
            .Where(
                x => x.PlanRoomGroups!.Any(
                    y => y.PlanId == planId
                )
            )
            .Where(
                x => x.PlanRoomGroupSites!.Any(
                    y => y.SiteId == siteId
                        && y.PlanId == planId
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
        long planId,
        long roomGroupId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = personAgeTypeRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x =>
                    x.PlanId == planId
                    && x.SiteId == siteId
                    && x.RoomGroupId == roomGroupId
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
