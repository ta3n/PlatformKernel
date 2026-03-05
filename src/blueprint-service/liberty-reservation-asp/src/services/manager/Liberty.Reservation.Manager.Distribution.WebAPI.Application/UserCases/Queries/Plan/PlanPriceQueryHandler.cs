using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Plan;

public class PlanPriceQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IPlanPriceService planPriceService,
    IFacilityPlanService facilityPlanService,
    ICheckFacilityService checkFacilityService
) : QuerySingleBaseHandler<PlanPriceQuery, BaseDataResponse<GetPriceDataPlanRoomResponse>>(mapper, cacheService)
{
    protected override string GetCacheKey(
        PlanPriceQuery request
    )
    {
        var compactUserCode = securityContextAccessor.CompactApplicationUserKey;

        var queryString = request.Request;

        return CacheHelper.GetCacheKeyByParameters(
            string.Format(
                CacheKeys.ExternalGetPlanPricePrefixKey,
                compactUserCode,
                string.Join(
                    "",
                    $"{queryString.ScAgtPlanCode}-{queryString.ScAgtSiteCode}-{queryString.ScAgtRoomCode}"
                )
            ),
            CacheHelper.ComputeHash(
                [
                    nameof(PlanPriceQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected async Task<bool> CheckValidRequest(
        PlanPriceQuery request,
        string[] facilityCodes,
        CancellationToken cancellationToken
    )
    {
        var userCode = securityContextAccessor.ApplicationUserKey ?? string.Empty;
        var facilityCodesIsValid = await checkFacilityService.CheckUserManagedFacilityAsync(
            facilityCodes,
            userCode,
            cancellationToken
        );

        return facilityCodesIsValid;
    }

    protected override async Task<(IHeaderDictionary, BaseDataResponse<GetPriceDataPlanRoomResponse>)> HandleAsync(
        PlanPriceQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityCodes = await facilityPlanService.GetFacilityCodeByPlanCodAsync(request.Request.ScAgtPlanCode!, cancellationToken);

        var facilityCodesArray = facilityCodes as string[] ?? facilityCodes.ToArray();
        var isValid = await CheckValidRequest(request, facilityCodesArray, cancellationToken);
        if (!isValid && facilityCodesArray.Length != 0)
        {
            throw new FacilityForbiddenException();
        }

        var (planId, roomGroupName, siteId, fromDateSearch, acquireDayNums) = request.Request;

        var toDate = AppDate.GetDateTime(long.Parse(fromDateSearch!)).AddDays(long.Parse(acquireDayNums!));
        var toDateSearch = AppDate.GetId(toDate);

        var planPrices = await planPriceService.GetPlanPriceTariffDataAsync(
            planId ?? string.Empty,
            siteId ?? string.Empty,
            roomGroupName ?? string.Empty,
            long.Parse(fromDateSearch!),
            toDateSearch,
            cancellationToken
        );
        return (
            new HeaderDictionary(),
            new BaseDataResponse<GetPriceDataPlanRoomResponse>
            {
                Data = new GetPriceDataPlanRoomResponse { TariffData = planPrices.ToList() }
            }
        );
    }
}
