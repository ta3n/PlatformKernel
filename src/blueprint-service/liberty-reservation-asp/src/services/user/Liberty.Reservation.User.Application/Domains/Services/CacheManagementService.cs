using Liberty.Cache.Services;
using Liberty.Reservation.User.Application.Domains.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.User.Application.Domains.Services;

public class CacheManagementService(
    ILogger<CacheManagementService> logger,
    ICacheService cacheService
) : ICacheManagementService
{
    public void RemoveFacilityRelatedCache(
        long? facilityId
    )
    {
        try
        {
            cacheService.RemoveByPatterns(
                true,
                $"*{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}*",
                $"*{string.Format(CacheKeys.DistributionHotelsPrefixKey, facilityId)}*"
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
