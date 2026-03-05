using Liberty.Cache.Services;
using Liberty.Entity;
using Liberty.Reservation.Manager.Application.Auth;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Reservation.Manager.Application.Contexts.Interceptors;

public class CacheSaveChangesInterceptor(
    IServiceProvider serviceProvider
) : SaveChangesInterceptor
{
    private readonly ICacheService _cacheService = serviceProvider.GetRequiredService<ICacheService>();

    private readonly ISecurityContextAccessor _securityContextAccessor =
        serviceProvider.GetRequiredService<ISecurityContextAccessor>();

    private readonly ILogger<CacheSaveChangesInterceptor> _logger =
        serviceProvider.GetRequiredService<ILogger<CacheSaveChangesInterceptor>>();

    private readonly ICacheManagementService _cacheManagementService =
        serviceProvider.GetRequiredService<ICacheManagementService>();

    public override int SavedChanges(
        SaveChangesCompletedEventData eventData,
        int result
    )
    {
        var context = eventData.Context;

        if (context == null)
        {
            return base.SavedChanges(eventData, result);
        }

        var entries = context.ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            ClearCacheEntry(entry);
        }

        _cacheManagementService.RemoveFacilityRelatedCache();

        _cacheManagementService.RemoveUserKeyRelatedCache();

        return base.SavedChanges(eventData, result);
    }

    public override ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = new()
    )
    {
        var context = eventData.Context;

        if (context == null)
        {
            return base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        var hasNoFacilityAndUserCode = _securityContextAccessor.GetFacilityCodeValue() is null
            && _securityContextAccessor.ApplicationUserKey is null;

        if (hasNoFacilityAndUserCode)
        {
            return base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        var entries = context.ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            ClearCacheEntry(entry);
        }

        _cacheManagementService.RemoveFacilityRelatedCache();

        _cacheManagementService.RemoveUserKeyRelatedCache();

        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private void ClearCacheEntry(
        EntityEntry entry
    )
    {
        try
        {
            var facilityId = _securityContextAccessor.FacilityKey;

            if (entry.Entity is not EntityData entity)
            {
                return;
            }

            var entityName = entity.GetType().Name;
            var cacheKeyEntityPattern = $"{entityName}:{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}*";

            _cacheService.RemoveByPatterns(
                false,
                cacheKeyEntityPattern
            );

            if (entity is Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation reservation)
            {
                _cacheManagementService.RemoveAllFacilityBookingCache(
                    reservation.Id
                );
            }

            if (entity is Facility)
            {
                var cacheKeyFacility = $"{CacheKeys.RssFacilityKeyPatternKey}{entity.Id}*";
                _cacheService.RemoveByPatterns(true, cacheKeyFacility);
            }

            if (entity is PersonAgeType)
            {
                var cacheKeyFacility = $"{CacheKeys.RssPersonAgeTypesPatternKey}";
                _cacheService.RemoveByPatterns(true, cacheKeyFacility);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to remove cache entries related to entity: {EntityName}",
                entry.Entity.GetType().Name
            );
        }
    }
}
