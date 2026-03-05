using Liberty.Cache.Services;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class CacheManagementService(
    ILogger<CacheManagementService> logger,
    ISecurityContextAccessor securityContextAccessor,
    ICacheService cacheService
) : ICacheManagementService
{
    public void RemoveFacilityRelatedCache()
    {
        var facilityId = 0L;

        try
        {
            facilityId = securityContextAccessor.FacilityKey;

            cacheService.RemoveByPatterns(
                true,
                $"*{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}*"
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to remove cache entries related to FacilityId: {FacilityId}",
                facilityId
            );
        }
    }

    public void RemoveUserKeyRelatedCache()
    {
        var appUserKey = "";

        try
        {
            appUserKey = securityContextAccessor.CompactApplicationUserKey ?? string.Empty;

            cacheService.RemoveByPatterns(
                true,
                $"*{string.Format(CacheKeys.KakusanGetAllBookingQueryPrefixKey, appUserKey, string.Empty)}*",
                $"*{string.Format(CacheKeys.KakusanGetAllRoomQueryPrefixKey, appUserKey, string.Empty)}*",
                $"*{string.Format(CacheKeys.KakusanGetAllRoomTypeQueryPrefixKey, appUserKey, string.Empty)}*",
                $"*{string.Format(CacheKeys.ExternalGetPlanPrefixKey, appUserKey, string.Empty)}*",
                $"*{string.Format(CacheKeys.ExternalGetPlanPricePrefixKey, appUserKey, string.Empty)}*"
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to remove cache entries related to UserKey: {UserKey}",
                appUserKey
            );
        }
    }

    public void RemoveAllFacilityBookingCache(
        long? bookingId
    )
    {
        try
        {
            cacheService.RemoveByPatterns(
                true,
                $"*{CacheKeys.AllFacilityBookingSearchPrefixKey}*"
            );

            if (bookingId is not null)
            {
                cacheService.RemoveByPatterns(
                    true,
                    $"*{string.Format(CacheKeys.AllFacilityBookingDetailPrefixKey, bookingId)}*"
                );
            }
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to remove all facility booking cache"
            );
        }
    }
}
