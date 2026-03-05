using System.Globalization;
using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Validations;
using Liberty.SysException;
using Microsoft.Extensions.Options;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Kakusan;

public class KakusanGetAllRoomQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IOptions<C002Setting> c002Options,
    IRoomGroupAppDateRepository roomGroupAppDateRepository,
    IFacilityRepository facilityRepository,
    ICheckFacilityService checkFacilityService
) : QuerySingleBaseHandler<KakusanGetAllRoomQuery, GetRoomsResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        KakusanGetAllRoomQuery request
    )
    {
        var compactUserCode = securityContextAccessor.CompactApplicationUserKey;

        var hotelCodes = request.Request.HotelIds.ToArray();
        var hotelIds = GetAllFacilityIds(hotelCodes);

        return CacheHelper.GetCacheKeyByParameters(
            string.Format(
                CacheKeys.KakusanGetAllRoomQueryPrefixKey,
                compactUserCode,
                string.Join(
                    "",
                    hotelIds.Select(x => string.Format(CacheKeys.DistributionHotelsPrefixKey, x))
                )
            ),
            CacheHelper.ComputeHash(
                [
                    nameof(KakusanGetAllRoomQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<bool> IsValidRequest(
        KakusanGetAllRoomQuery request,
        CancellationToken cancellationToken
    )
    {
        var userCode = securityContextAccessor.ApplicationUserKey ?? string.Empty;

        var hotelCodes = request.Request.HotelIds.ToArray();
        var hotelIds = GetAllFacilityIds(hotelCodes);

        var hotelIdsIsValid = await checkFacilityService.CheckUserManagedFacilityAsync(
            hotelCodes,
            userCode,
            cancellationToken
        );

        var validation = await new KakusanGetAllRoomQueryValidator(c002Options.Value).ValidateAsync(
            request,
            cancellationToken
        );

        return validation.IsValid && hotelIdsIsValid && hotelCodes.Length == hotelIds.Count;
    }

    protected override async Task<(IHeaderDictionary, GetRoomsResponse)> HandleAsync(
        KakusanGetAllRoomQuery query,
        CancellationToken cancellationToken
    )
    {
        var request = query.Request;
        var hotels = new List<HotelDataResponse>();

        var isValid = await IsValidRequest(
            query,
            cancellationToken
        );
        if (!isValid)
        {
            return (
                new HeaderDictionary(),
                new GetRoomsResponse
                {
                    Hotels = [],
                    ErrorCode = ErrorCode.E4001
                }
            );
        }

        foreach (var value in request.HotelIds)
        {
            var queryable = roomGroupAppDateRepository
                .GetQueryableWithAsNoTracking()
                .Where(
                    x => x.RoomGroup!.FacilityRoomGroups!.Any(
                        t => t.Facility!.Code == value
                    )
                )
                .Where(
                    x => x.AppDateId >= request.FromAppDateId
                        && x.AppDateId <= request.ToAppDateId
                )
                .Where(
                    x => x.RoomGroup!.GroupName != null
                        && x.RoomGroup.GroupName != string.Empty
                )
                .Select(
                    x => new RoomGroupAppDateResponse
                    {
                        AppDateId = x.AppDateId,
                        RoomGroupId = x.RoomGroup!.GroupName,
                        RoomGroupName = x.RoomGroup!.Name!.GetValueByHeader(),
                        SellNumber = x.SellNumber ?? 0,
                        IsNotSold = x.IsNotSelled,
                        ReservedNumber = x.RoomGroup!.ReservationPlanRoomGroupAppDates!
                            .Where(t => t.BookingDateId == x.AppDateId)
                            .Count(
                                t =>
                                    t.Reservation!.ReservationState == ReservationStatus.Confirmed
                                    || t.Reservation!.ReservationState == ReservationStatus.Reserved
                                    || t.Reservation!.ReservationState == ReservationStatus.Modified
                            )
                    }
                );

            var listRoomGroupAppDateResponse = await queryable.ToListAsync(cancellationToken);

            foreach (var roomGroupAppDateResponse in listRoomGroupAppDateResponse)
            {
                roomGroupAppDateResponse.RemainNumber = roomGroupAppDateResponse.SellNumber - roomGroupAppDateResponse.ReservedNumber;
            }

            var rooms = listRoomGroupAppDateResponse
                .OrderBy(y => y.AppDateId)
                .GroupBy(x => x.RoomGroupId)
                .Select(
                    group => new Models.Responses.Room
                    {
                        RoomId = group.Key,
                        Dates =
                        [
                            .. group.Select(
                                item => new Date
                                {
                                    TargetDate = DateTime
                                        .ParseExact(
                                            item.AppDateId.ToString(),
                                            "yyyyMMdd",
                                            CultureInfo.InvariantCulture
                                        )
                                        .ToString("yyyy-MM-dd"),
                                    TotalRoomCount = item.SellNumber,
                                    VacantCount = item.RemainNumber,
                                    CloseType = item.IsNotSold ? EnumCloseType.Stop : EnumCloseType.Sale
                                }
                            )
                        ]
                    }
                )
                .OrderBy(x => x.RoomId)
                .ToList();

            hotels.Add(
                new HotelDataResponse
                {
                    HotelId = value,
                    Rooms = rooms
                }
            );
        }

        return (
            new HeaderDictionary(),
            new GetRoomsResponse { Hotels = hotels }
        );
    }

    private List<long> GetAllFacilityIds(
        string[] facilityCodes
    )
    {
        var hotelIds = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => facilityCodes.Contains(x.Code!) && x.IsEnabled)
            .Select(x => x.Id)
            .ToList();

        return hotelIds;
    }
}
