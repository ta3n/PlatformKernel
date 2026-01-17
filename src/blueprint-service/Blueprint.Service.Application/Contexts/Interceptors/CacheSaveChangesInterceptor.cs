using Blueprint.Service.Application.Auth;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Cache.Services;
using SharedKernel.Entity;

namespace Blueprint.Service.Application.Contexts.Interceptors;

public class CacheSaveChangesInterceptor(
    IServiceProvider serviceProvider
) : SaveChangesInterceptor
{
    private readonly ICacheService _cacheService = serviceProvider.GetRequiredService<ICacheService>();

    private readonly ISecurityContextAccessor _securityContextAccessor =
        serviceProvider.GetRequiredService<ISecurityContextAccessor>();

    private readonly ILogger<CacheSaveChangesInterceptor> _logger =
        serviceProvider.GetRequiredService<ILogger<CacheSaveChangesInterceptor>>();

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

        var hasNoUserCode = _securityContextAccessor.ApplicationUserKey is null;

        if (hasNoUserCode)
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

            _cacheService.RemoveByPatterns(
                false,
                cacheKeyEntityPattern
            );
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
