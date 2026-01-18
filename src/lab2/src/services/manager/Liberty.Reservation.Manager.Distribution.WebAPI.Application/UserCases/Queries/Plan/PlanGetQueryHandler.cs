using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Plan;

public class PlanGetQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    ICheckFacilityService checkFacilityService,
    IPlanPriceService planPriceService,
    IFacilityService facilityService
) : QuerySingleBaseHandler<PlanGetQuery, BaseDataResponse<GetPlanResponse>>(mapper, cacheService)
{
    protected override string GetCacheKey(
        PlanGetQuery request
    )
    {
        var compactUserCode = securityContextAccessor.CompactApplicationUserKey;

        var facilityCodes = request.Request.ScAgtFacilityCode ?? string.Empty;

        return CacheHelper.GetCacheKeyByParameters(
            string.Format(
                CacheKeys.ExternalGetPlanPrefixKey,
                compactUserCode,
                string.Join(
                    "",
                    facilityCodes
                )
            ),
            CacheHelper.ComputeHash(
                [
                    nameof(PlanGetQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, BaseDataResponse<GetPlanResponse>)> HandleAsync(
        PlanGetQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityCode = request.Request.ScAgtFacilityCode ?? string.Empty;
        var userCode = securityContextAccessor.ApplicationUserKey ?? string.Empty;

        var facilityId = await facilityService.GetFacilityIdByCodeAsync(
            facilityCode,
            cancellationToken
        );
        if (facilityId <= 0)
        {
            throw new FieldExternalPlanInvalidException(nameof(GetPlanRequest.ScAgtFacilityCode), facilityCode);
        }

        var facilityCodesIsValid = await checkFacilityService.CheckUserManagedFacilityAsync(
            facilityCode,
            userCode,
            cancellationToken
        );

        if (!facilityCodesIsValid)
        {
            throw new FacilityForbiddenException();
        }

        var planRoomGroups = await planPriceService.GetPlanPriceAsync(
            facilityCode,
            cancellationToken
        );

        return (
            new HeaderDictionary(),
            new BaseDataResponse<GetPlanResponse> { Data = new GetPlanResponse { AgtPlanRoomInfos = [.. planRoomGroups] } }
        );
    }
}
