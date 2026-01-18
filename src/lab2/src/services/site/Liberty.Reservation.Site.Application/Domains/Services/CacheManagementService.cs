using Liberty.Cache.Services;
using Liberty.Reservation.Site.Application.Auth;

namespace Liberty.Reservation.Site.Application.Domains.Services;

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
            facilityId = securityContextAccessor.GetFacilityIdSelected();

            cacheService.RemoveByPatterns(
                true,
                $"*{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}*",
                $"*{string.Format(CacheKeys.DistributionHotelsPrefixKey, "*")}"
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
