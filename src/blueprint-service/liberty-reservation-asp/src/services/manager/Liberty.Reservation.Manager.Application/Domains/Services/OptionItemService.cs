using Liberty.ApplicationShared.Utils;
using Liberty.Cache.Services;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class OptionItemService(
    ILogger<OptionItemService> logger,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IOptionItemRepository optionItemRepository
) : BaseService<OptionItem>(logger, cacheService, optionItemRepository, new OptionItemNotfoundException()),
    IOptionItemService
{
    protected override string GetCacheKey(
        string methodName = "",
        params string[] keys
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;
        return $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}:" + base.GetCacheKey(methodName, keys);
    }

    protected override IQueryable<OptionItem> GetQueryable()
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return optionItemRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.FacilityOptionItems!.Any(
                    t => t.FacilityId == facilityId
                )
            );
    }

    public override Task<OptionItem> CreateAsync(
        OptionItem entityToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        entityToCreate.Code = EntityUtil.CreateCode();
        entityToCreate.RecordMemo = EntityUtil.CreateRecordMemo();

        return base.CreateAsync(
            entityToCreate,
            autoSave,
            cancellationToken
        );
    }

    public override Task<OptionItem> UpdateAsync(
        OptionItem entityToUpdate,
        bool autoSave = true,
        Func<OptionItem, OptionItem, OptionItem>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.Name ??= [];
                existingEntity.Name?.UpdateLocalized(updateEntity.Name);
                existingEntity.Description = updateEntity.Description;
                existingEntity.BaseNumber = updateEntity.BaseNumber;
                existingEntity.Price = updateEntity.Price;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public async Task<long> CountAvailableByIdsAsync(
        long[] ids,
        DateTime dateCheck,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = GetQueryable()
            .Where(x => ids.Contains(x.Id))
            .Where(x => x.IsEnabled)
            .Where(x => x.EnabledStart == null || x.EnabledStart <= dateCheck)
            .Where(x => x.EnabledEnd == null || x.EnabledEnd >= dateCheck)
            .Where(x => x.UseDisplayDate || x.DisplayDateStart == null || x.DisplayDateStart <= dateCheck)
            .Where(x => x.UseDisplayDate || x.DisplayDateEnd == null || x.DisplayDateEnd >= dateCheck)
            .Where(x => x.UseAcceptDate || x.AcceptDateStart == null || x.AcceptDateStart <= dateCheck)
            .Where(x => x.UseAcceptDate || x.AcceptDateEnd == null || x.AcceptDateEnd >= dateCheck);

        var existingCount = await queryable.CountAsync(
            cancellationToken
        );

        return existingCount;
    }

    public async Task<IEnumerable<OptionItemAppDate>> FindAllAvailableAppDatesOfOptionItemsAsync(
        long[] optionItemIds,
        long startAppDateCheckId,
        long endAppDateCheckId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = GetQueryable()
            .Where(x => optionItemIds.Contains(x.Id))
            .SelectMany(x => x.OptionItemAppDates!)
            .Where(x => x.AppDateId >= startAppDateCheckId)
            .Where(x => x.AppDateId <= endAppDateCheckId)
            .OrderBy(x => x.AppDateId);

        var data = await queryable.ToListAsync(
            cancellationToken
        );

        return data;
    }

    public async Task UpdateLastModifiedAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        var longDate = ConvertUtil.ToLong(
            ConvertUtil.ToString(DateTime.UtcNow, "yyyyMMddHHmmssfff")
        );

        await optionItemRepository
            .GetQueryable()
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(b => b.UpdatedAt, longDate),
                cancellationToken
            );
    }
}
