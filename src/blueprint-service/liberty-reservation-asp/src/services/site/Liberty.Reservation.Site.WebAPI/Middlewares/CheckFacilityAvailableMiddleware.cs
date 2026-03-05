using Liberty.ApplicationShared.Settings;
using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Site.Application.Auth;
using Liberty.Reservation.Site.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Site.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.ServiceDefaults.Middlewares;
using Liberty.SysException.Exceptions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Liberty.Reservation.Site.WebAPI.Middlewares;

public class CheckFacilityAvailableMiddleware(
    IOptions<AppInfo> appInfoOptions,
    ICacheService? cacheService,
    IFacilityExternalRepository facilityExternalRepository,
    IFacilityService facilityService
) : BaseMiddleware
{
    private const int HashLength = 5;
    private const int CacheExpirationMinutes = 5;
    private const string JsonContentType = "application/json";

    private static readonly string CachedMiddlewareNameHash = CacheHelper.ComputeHash(
        [
            nameof(CheckFacilityAvailableMiddleware)
        ]
    );

    protected override async Task HandleAsync(
        HttpContext context,
        RequestDelegate next
    )
    {
        var isFacilityKeyExist = context.Request.Headers.TryGetValue(
            SecurityContextAccessor.FacilityCodeHeaderKey,
            out var facilityCodeValue
        );
        var facilityCode = facilityCodeValue.ToString().Trim();
        var facilityKeyValid = !string.IsNullOrEmpty(facilityCode);

        if (!isFacilityKeyExist || !facilityKeyValid)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync($"Error: '{SecurityContextAccessor.FacilityCodeHeaderKey}' header is missing.");
            return;
        }

        var isSiteKeyExist = context.Request.Headers.TryGetValue(
            SecurityContextAccessor.SiteCodeOfFacilityHeaderKey,
            out var siteCodeValue
        );
        var siteCode = siteCodeValue.ToString().Trim();
        var siteKeyValid = !string.IsNullOrEmpty(siteCode);

        if (!isSiteKeyExist || !siteKeyValid)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync($"Error: '{SecurityContextAccessor.SiteCodeOfFacilityHeaderKey}' header is missing.");
            return;
        }

        var (facilityId, siteId, facilityRecordCode, isAvailable) = await CheckFacilityAvailableAsync(
            facilityCode,
            siteCode
        );

        if (isAvailable)
        {
            context.Request.Headers.Append(
                SecurityContextAccessor.FacilityIdHeaderKey,
                $"{facilityId}"
            );

            context.Request.Headers.Append(
                SecurityContextAccessor.SiteIdOfFacilityHeaderKey,
                $"{siteId}"
            );

            context.Request.Headers.Append(
                SecurityContextAccessor.FacilityHeaderRecordCode,
                $"{facilityRecordCode}"
            );

            await next(context);
            return;
        }

        var appInfo = appInfoOptions.Value;
        var appException = new FacilityNotAvailableException();
        var exceptionObject = new
        {
            // Exception
            code = appException.ErrorCode.ToString(),
            title = appException.Title,
            description = appException.Message,
            app = new
            {
                name = appInfo.AppName,
                version = appInfo.AppVersion,
                date = $"{DateTime.UtcNow}"
            }
        };

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = JsonContentType;
        await context.Response.WriteAsJsonAsync(exceptionObject);
    }

    private async Task<FacilityCheck> CheckFacilityAvailableAsync(
        string facilityCode,
        string siteCode
    )
    {
        var isCache = cacheService is not null && cacheService.IsEnabled;
        var cacheKey = string.Empty;

        if (isCache)
        {
            cacheKey = CacheHelper.GetCacheKeyByParameters(
                string.Format(
                    CacheKeys.BookingSearchPrefixKey,
                    CacheHelper.ComputeHash(facilityCode, HashLength),
                    CacheHelper.ComputeHash(siteCode, HashLength)
                ),
                CachedMiddlewareNameHash
            );

            var dataCache = await cacheService!.GetAsync<FacilityCheck>(
                cacheKey
            );

            if (dataCache is not null)
            {
                return dataCache;
            }
        }

        var facilityInfoDto = await facilityExternalRepository.GetFacilityAvailableByCodeAsync(facilityCode);
        var isFacilityAvailableOfMembership = facilityInfoDto is not null;

        var (isSiteAvailableOfFacility, facilityId, siteId) = await facilityService.CheckSiteCodeAlreadyInFacilityAsync(
            facilityCode,
            siteCode
        );
        var isAvailable = isFacilityAvailableOfMembership && isSiteAvailableOfFacility;

        var facilityCheck = new FacilityCheck(
            facilityId,
            siteId,
            facilityInfoDto?.RecordCode,
            isAvailable
        );

        if (isCache)
        {
            await cacheService!.SetAsync(
                cacheKey,
                facilityCheck,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(
                        CacheExpirationMinutes
                    )
                }
            );
        }

        return facilityCheck;
    }

    private sealed record FacilityCheck(
        long FacilityId,
        long SiteId,
        string? FacilityRecordCode,
        bool IsAvailable
    );
}
