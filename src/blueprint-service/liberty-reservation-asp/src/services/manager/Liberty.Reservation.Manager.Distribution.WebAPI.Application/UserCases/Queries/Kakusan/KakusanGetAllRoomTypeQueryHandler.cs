using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Validations;
using Liberty.SysException;
using Microsoft.Extensions.Options;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Kakusan;

public class KakusanGetAllRoomTypeQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IOptions<C001Setting> c001Options,
    IFacilityRepository facilityRepository,
    ICheckFacilityService checkFacilityService
) : QuerySingleBaseHandler<KakusanGetAllRoomTypeQuery, RoomTypeResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        KakusanGetAllRoomTypeQuery request
    )
    {
        var compactUserCode = securityContextAccessor.CompactApplicationUserKey;

        var hotelCodes = request.Request.HotelIds.ToArray();
        var hotelIds = GetAllFacilityIds(hotelCodes);

        return CacheHelper.GetCacheKeyByParameters(
            string.Format(
                CacheKeys.KakusanGetAllRoomTypeQueryPrefixKey,
                compactUserCode,
                string.Join(
                    "",
                    hotelIds.Select(x => string.Format(CacheKeys.DistributionHotelsPrefixKey, x))
                )
            ),
            CacheHelper.ComputeHash(
                [
                    nameof(KakusanGetAllRoomTypeQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<bool> IsValidRequest(
        KakusanGetAllRoomTypeQuery request,
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

        var validation = await new KakusanGetAllRoomTypeQueryValidator(c001Options.Value).ValidateAsync(
            request,
            cancellationToken
        );

        return validation.IsValid && hotelIdsIsValid && hotelCodes.Length == hotelIds.Count;
    }

    protected override async Task<(IHeaderDictionary, RoomTypeResponse)> HandleAsync(
        KakusanGetAllRoomTypeQuery query,
        CancellationToken cancellationToken
    )
    {
        var request = query.Request;

        var isValid = await IsValidRequest(
            query,
            cancellationToken
        );
        if (!isValid)
        {
            return (
                new HeaderDictionary(),
                new RoomTypeResponse
                {
                    Hotels = [],
                    ErrorCode = ErrorCode.E4001
                }
            );
        }

        var queryable = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => request.HotelIds.Contains(x.Code!))
            .Select(
                facility => new Hotel
                {
                    Id = facility.Code,
                    Rooms = facility.FacilityRoomGroups!
                        .Where(
                            x => x.RoomGroup!.GroupName != null
                                && x.RoomGroup!.GroupName != string.Empty
                        )
                        .Select(
                            roomGroup => new RoomResponse
                            {
                                RoomId = roomGroup.RoomGroup!.GroupName,
                                Name = roomGroup.RoomGroup.Name!.GetValueByHeader(DefaultValues.LanguageCode),
                                Rooms = roomGroup.RoomGroup.BaseNumber,
                                People = roomGroup.RoomGroup.CapacityMax ?? 0
                            }
                        )
                        .ToList()
                }
            );

        var facilities = await queryable.ToListAsync(cancellationToken);

        return (
            new HeaderDictionary(),
            new RoomTypeResponse { Hotels = facilities }
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
