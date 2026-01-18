using Liberty.Cache.Services;
using Liberty.Entity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Reservation.Employee.Application.Contexts.Interceptors;

public class CacheSaveChangesInterceptor(
    IServiceProvider serviceProvider
) : SaveChangesInterceptor
{
    private readonly ICacheService _cacheService = serviceProvider.GetRequiredService<ICacheService>();

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

        var entries = context.ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            ClearCacheEntry(entry);
        }

        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private void ClearCacheEntry(
        EntityEntry entry
    )
    {
        try
        {
            if (entry.Entity is not EntityData entity)
            {
                return;
            }

            var entityName = entity.GetType().Name;
            var cacheKeyEntityPattern = $"{entityName}*";

            _cacheService.Reset(cacheKeyEntityPattern);
            if (entity is Facility)
            {
                var cacheKeyFacility = $"{CacheKeys.RssFacilityKeyPatternKey}{entity.Id}*";
                _cacheService.RemoveByPatterns(true, cacheKeyFacility);
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
