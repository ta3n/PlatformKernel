using Liberty.ApplicationShared.Utils;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class SiteService(
    ILogger<SiteService> logger,
    ICacheService cacheService,
    ISiteRepository siteRepository
) : BaseService<Site>(logger, cacheService, siteRepository, new SiteNotfoundException()), ISiteService
{
    public override Task<Site> CreateAsync(
        Site entityToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        entityToCreate.Code = EntityUtil.CreateCode();
        entityToCreate.RecordMemo = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        entityToCreate.IsEnabled = false;

        return base.CreateAsync(
            entityToCreate,
            autoSave,
            cancellationToken
        );
    }

    public override Task<Site> UpdateAsync(
        Site entityToUpdate,
        bool autoSave = true,
        Func<Site, Site, Site>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? (
                (
                    existingEntity,
                    updateEntity
                ) =>
                {
                    existingEntity.Code = updateEntity.Code;
                    existingEntity.Name ??= [];
                    existingEntity.Name?.UpdateLocalized(updateEntity.Name);
                    existingEntity.ShortName = updateEntity.ShortName;
                    existingEntity.PrefixName = updateEntity.PrefixName;
                    existingEntity.Url = updateEntity.Url;
                    existingEntity.UseSitePoint = updateEntity.UseSitePoint;
                    existingEntity.Meta = updateEntity.Meta;

                    return existingEntity;
                }
            );

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public Task<bool> CheckExistingCode(
        string code,
        IEnumerable<long> ignoreIds,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = siteRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Code == code)
            .Where(x => !ignoreIds.Contains(x.Id));

        var result = queryable.AnyAsync(cancellationToken);

        return result;
    }

    public Task<bool> CheckExistingPrefixName(
        string prefixName,
        IEnumerable<long> ignoreIds,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = siteRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.PrefixName == prefixName)
            .Where(x => !ignoreIds.Contains(x.Id));

        var result = queryable.AnyAsync(cancellationToken);

        return result;
    }
}
