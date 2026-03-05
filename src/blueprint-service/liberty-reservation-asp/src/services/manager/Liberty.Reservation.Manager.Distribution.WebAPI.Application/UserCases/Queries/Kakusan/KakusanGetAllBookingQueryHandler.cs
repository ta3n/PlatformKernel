using AutoMapper.QueryableExtensions;
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
using static Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests.GetBookingRequest;
using static Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses.GetBookingResponse;
using Hotel = Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses.GetBookingResponse.Hotel;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Kakusan;

public class KakusanGetAllBookingQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IOptions<C003Setting> c003Options,
    IReservationRepository reservationRepository,
    IFacilityRepository facilityRepository,
    ICheckFacilityService checkFacilityService
) : QuerySingleBaseHandler<KakusanGetAllBookingQuery, GetBookingResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        KakusanGetAllBookingQuery request
    )
    {
        var compactUserCode = securityContextAccessor.CompactApplicationUserKey;

        var hotelCodes = request.Request.HotelIds.ToArray();
        var hotelIds = GetAllFacilityIds(hotelCodes);

        return CacheHelper.GetCacheKeyByParameters(
            string.Format(
                CacheKeys.KakusanGetAllBookingQueryPrefixKey,
                compactUserCode,
                string.Join(
                    "",
                    hotelIds.Select(x => string.Format(CacheKeys.DistributionHotelsPrefixKey, x))
                )
            ),
            CacheHelper.ComputeHash(
                [
                    nameof(KakusanGetAllBookingQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<bool> IsValidRequest(
        KakusanGetAllBookingQuery request,
        CancellationToken cancellationToken
    )
    {
        var userCode = securityContextAccessor.ApplicationUserKey ?? string.Empty;

        var hotelCodes = request.Request.HotelIds.ToArray();
        var hotelIds = GetAllFacilityIds(hotelCodes);

        var hotelCodesIsValid = await checkFacilityService.CheckUserManagedFacilityAsync(
            hotelCodes,
            userCode,
            cancellationToken
        );

        var validation = await new KakusanGetAllBookingQueryValidator(c003Options.Value).ValidateAsync(
            request,
            cancellationToken
        );

        return validation.IsValid && hotelCodesIsValid && hotelCodes.Length == hotelIds.Count;
    }

    protected override async Task<(IHeaderDictionary, GetBookingResponse)> HandleAsync(
        KakusanGetAllBookingQuery query,
        CancellationToken cancellationToken
    )
    {
        var request = query.Request;
        var facilityIds = request.HotelIds;

        var isValid = await IsValidRequest(
            query,
            cancellationToken
        );
        if (!isValid)
        {
            return (
                new HeaderDictionary(),
                new GetBookingResponse
                {
                    Hotels = [],
                    ErrorCode = ErrorCode.E4001
                }
            );
        }

        if (request.DateType is EnumDataType.CheckIn or EnumDataType.NoShow)
        {
            return (
                new HeaderDictionary(),
                new GetBookingResponse { Hotels = [] }
            );
        }

        var queryable = BuildQueryable(
            request.DateType,
            request.GetFromDay(),
            request.GetToDay(),
            request.GetFromArriveDay(),
            request.GetToArriveDay(),
            facilityIds
        );

        var bookingQueryable = queryable
            .GroupBy(x => x.FacilityCode)
            .Select(
                x => new Hotel
                {
                    HotelId = x.Key,
                    Bookings = x.ToList()
                }
            );

        var data = await bookingQueryable.ToListAsync(cancellationToken);

        return (new HeaderDictionary(), new GetBookingResponse { Hotels = data });
    }

    private IQueryable<Booking> BuildQueryable(
        EnumDataType? dateType,
        DateTime fromDay,
        DateTime toDay,
        DateTime? fromArriveDay,
        DateTime? toArriveDay,
        List<string> facilityCodes
    )
    {
        var utcFromDay = fromDay.AddHours(-1 * DefaultValues.TimeZoneOffset);
        var utcToDay = toDay.AddHours(-1 * DefaultValues.TimeZoneOffset);

        var queryable = reservationRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => facilityCodes.Contains(x.Facility!.Code!));

        switch (dateType)
        {
            case EnumDataType.ReserveOrCancel:
                // 0：予約受付またはキャンセル（NoShowを含む）受付
                queryable = queryable.Where(
                    x =>
                        ((x.ReservationState == ReservationStatus.Confirmed
                                || x.ReservationState == ReservationStatus.Reserved)
                            && x.ConfirmedDateTime >= utcFromDay
                            && x.ConfirmedDateTime <= utcToDay)
                        || ((x.ReservationState == ReservationStatus.UserCanceled
                                || x.ReservationState == ReservationStatus.GuestCanceled
                                || x.ReservationState == ReservationStatus.ManagerCanceled)
                            && x.CancelledDateTime >= utcFromDay
                            && x.CancelledDateTime <= utcToDay)
                        || (x.ReservationState == ReservationStatus.Modified
                            && x.ReservationDateTime >= utcFromDay
                            && x.ReservationDateTime <= utcToDay)
                );
                break;
            case EnumDataType.Reserve:
                // 2：予約受付のみ(キャンセル除く、NoShow)
                queryable = queryable.Where(
                    x =>
                        ((x.ReservationState == ReservationStatus.Confirmed
                                || x.ReservationState == ReservationStatus.Reserved)
                            && x.ConfirmedDateTime >= utcFromDay
                            && x.ConfirmedDateTime <= utcToDay)
                        || (x.ReservationState == ReservationStatus.Modified
                            && x.ReservationDateTime >= utcFromDay
                            && x.ReservationDateTime <= utcToDay)
                );
                break;
            case EnumDataType.Cancel:
                // 3：キャンセル受付のみ
                queryable = queryable
                    .Where(
                        x => x.ReservationState == ReservationStatus.UserCanceled
                            || x.ReservationState == ReservationStatus.GuestCanceled
                            || x.ReservationState == ReservationStatus.ManagerCanceled
                    )
                    .Where(x => x.CancelledDateTime >= utcFromDay)
                    .Where(x => x.CancelledDateTime <= utcToDay);
                break;
        }

        if (fromArriveDay != null && toArriveDay != null)
        {
            var utcFromArriveDay = fromArriveDay.Value;
            var utcToArriveDay = toArriveDay.Value;
            var utcFromArriveDayLong = AppDate.GetId(utcFromArriveDay);
            var utcToArriveDayLong = AppDate.GetId(utcToArriveDay);
            var utcFromArriveDayTime = utcFromArriveDay.TimeOfDay;
            var utcToArriveDayTime = utcToArriveDay.TimeOfDay;
            if (utcFromArriveDayLong == utcToArriveDayLong)
            {
                queryable = queryable.Where(
                    x =>
                        x.CheckInDate == utcFromArriveDayLong
                        && x.CheckInTime >= utcFromArriveDayTime
                        && x.CheckInTime <= utcToArriveDayTime
                );
            }
            else
            {
                queryable = queryable.Where(
                    x =>
                        (x.CheckInDate > utcFromArriveDayLong && x.CheckInDate < utcToArriveDayLong)
                        || (x.CheckInDate == utcFromArriveDayLong && x.CheckInTime >= utcFromArriveDayTime)
                        || (x.CheckInDate == utcToArriveDayLong && x.CheckInTime <= utcToArriveDayTime)
                );
            }
        }

        return queryable.ProjectTo<Booking>(Mapper.ConfigurationProvider).AsSplitQuery();
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
