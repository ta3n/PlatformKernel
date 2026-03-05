using Liberty.Cache.Services;
using Liberty.Entity;
using Liberty.Reservation.User.Application.Domains.Services.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.User.Application.Contexts.Interceptors;

public class CacheSaveChangesInterceptor(
    IServiceProvider serviceProvider
) : SaveChangesInterceptor
{
    private readonly ICacheService _cacheService = serviceProvider.GetRequiredService<ICacheService>();

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

            if (entry.Entity is not Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation booking)
            {
                return;
            }

            _cacheManagementService.RemoveFacilityRelatedCache(booking.FacilityId);
            _cacheManagementService.RemoveAllFacilityBookingCache(booking.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while clearing cache for entity {EntityName}.",
                entry.Entity.GetType().Name
            );
        }
    }
}
