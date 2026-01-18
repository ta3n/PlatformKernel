using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Reservation.Site.Application.Auth;
using Liberty.Reservation.Site.Application.Exceptions;
using Liberty.Reservation.Site.Application.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Liberty.Reservation.Site.Application.Domains.Services;

public class CheckChangedService(
    IDbContextFactory<SiteDataContext> dbContextFactory,
    ISecurityContextAccessor securityContextAccessor,
    ICacheService cacheService
) : ICheckChangedService
{
    public async Task<LastUpdatedTimeOfBoookingModel> GetLastUpdatedAtAsync(
        long planId,
        long roomId,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.GetFacilityIdSelected();
        var siteId = securityContextAccessor.GetSiteIdSelected();

        var isCache = cacheService.IsEnabled;
        var cacheKey = string.Empty;

        if (isCache)
        {
            cacheKey = CacheHelper.GetCacheKeyByParameters(
                string.Format(
                    CacheKeys.BookingDetailPrefixKey,
                    facilityId,
                    siteId,
                    planId,
                    roomId
                ),
                CacheHelper.ComputeHash(
                    [nameof(GetLastUpdatedAtAsync)]
                )
            );

            var dataCache = await cacheService.GetAsync<LastUpdatedTimeOfBoookingModel>(
                cacheKey,
                cancellationToken
            );

            if (dataCache is not null)
            {
                return dataCache;
            }
        }

        // Execute tasks in parallel and store the results
        var getPlanUpdateTimeTask = GetPlanUpdateTimeAsync(
            planId,
            roomId,
            facilityId,
            siteId,
            cancellationToken
        );

        var getFacilityUpdateTimeTask = GetFacilityUpdateTimeAsync(
            facilityId,
            cancellationToken
        );

        var getRoomGroupUpdateTimeTask = GetRoomGroupUpdateTimeAsync(
            planId,
            roomId,
            siteId,
            cancellationToken
        );

        var getPersonAgeTypeUpdateTimeTask = GetPersonAgeTypeUpdateTimeAsync(
            planId,
            roomId,
            siteId,
            cancellationToken
        );

        await Task.WhenAll(
            getPlanUpdateTimeTask,
            getFacilityUpdateTimeTask,
            getRoomGroupUpdateTimeTask,
            getPersonAgeTypeUpdateTimeTask
        );

        var updateAtDto = getPlanUpdateTimeTask.Result;
        var facilityUpdatedAt = getFacilityUpdateTimeTask.Result;
        var roomGroupUpdatedAt = getRoomGroupUpdateTimeTask.Result;
        var personAgeTypeUpdatedAt = getPersonAgeTypeUpdateTimeTask.Result;

        // Assign additional values
        updateAtDto.FacilityUpdatedAt = facilityUpdatedAt;
        updateAtDto.RoomGroupUpdatedAt = roomGroupUpdatedAt;
        updateAtDto.PersonAgeTypeUpdatedAt = personAgeTypeUpdatedAt;

        if (isCache)
        {
            await cacheService.SetAsync(
                cacheKey,
                updateAtDto,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) },
                cancellationToken
            );
        }

        return updateAtDto;
    }

    private async Task<LastUpdatedTimeOfBoookingModel> GetPlanUpdateTimeAsync(
        long planId,
        long roomId,
        long facilityId,
        long siteId,
        CancellationToken cancellationToken
    )
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var queryable = dbContext.Set<Plan>()
            .AsNoTracking()
            .Where(
                x => x.PlanRoomGroupSites!.Any(
                    y => y.SiteId == siteId
                        && y.RoomGroupId == roomId
                        && x.IsEnabled
                )
            )
            .Where(
                x => x.Id == planId
                    && x.IsEnabled
                    && x.FacilityPlans!
                        .Any(y => y.IsEnabled && y.FacilityId == facilityId)
            )
            .Where(
                x => x.PlanRoomGroups!
                    .Any(
                        y => y.RoomGroupId == roomId
                            && y.RoomGroup!.IsEnabled
                    )
            )
            .Select(
                x => new LastUpdatedTimeOfBoookingModel
                {
                    PlanUpdatedAt = x.UpdatedAt,
                    SiteUpdatedAt = x.PlanSites != null && x.PlanSites.Count != 0
                        ? x.PlanSites.First(t => t.SiteId == siteId).Site!.UpdatedAt
                        : null,
                    CancellationUpdatedAt = x.Cancellation != null ? x.Cancellation!.UpdatedAt : null,
                    QuestionsUpdatedAt = x.PlanQuestions != null && x.PlanQuestions.Count != 0
                        ? x.PlanQuestions!.Select(t => t.Question!.UpdatedAt).ToList()
                        : null,
                    FilesUpdatedAt = x.FilePlans != null && x.FilePlans.Count != 0
                        ? x.FilePlans!.Select(t => t.UpdatedAt).ToList()
                        : null
                }
            )
            .AsSingleQuery();

        var plan = await queryable.FirstOrDefaultAsync(
            cancellationToken
        );

        return plan ?? throw new BookingNotfoundException();
    }

    private async Task<long> GetFacilityUpdateTimeAsync(
        long facilityId,
        CancellationToken cancellationToken
    )
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.Set<Facility>()
                .AsNoTracking()
                .Where(x => x.Id == facilityId)
                .Select(x => x.UpdatedAt)
                .FirstOrDefaultAsync(cancellationToken)
            ?? 0;
    }

    private async Task<long> GetRoomGroupUpdateTimeAsync(
        long planId,
        long roomId,
        long siteId,
        CancellationToken cancellationToken
    )
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var queryable = dbContext.Set<RoomGroup>()
            .AsNoTracking()
            .Where(
                x => x.Id == roomId
                    && x.IsEnabled
            )
            .Where(
                x => x.PlanRoomGroups!
                    .Any(y => y.PlanId == planId)
            )
            .Where(
                x => x.PlanRoomGroupSites!.Any(
                    y => y.SiteId == siteId
                        && y.PlanId == planId
                        && x.IsEnabled
                )
            )
            .Select(x => x.UpdatedAt);

        var roomGroupUpdatedAt = await queryable.FirstOrDefaultAsync(
                cancellationToken
            )
            ?? throw new RoomGroupNotfoundException();

        return roomGroupUpdatedAt;
    }

    private async Task<List<long?>> GetPersonAgeTypeUpdateTimeAsync(
        long planId,
        long roomId,
        long siteId,
        CancellationToken cancellationToken
    )
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var queryable = dbContext.Set<PlanRoomGroupSitePersonAgeType>()
            .AsNoTracking()
            .Where(
                x =>
                    x.PlanId == planId
                    && x.SiteId == siteId
                    && x.RoomGroupId == roomId
                    && x.IsEnabled
                    && x.PersonAgeType!.IsEnabled
                    && x.PersonAgeType!.IsVisible
            )
            .Select(
                x => x.PersonAgeType!.UpdatedAt
            );

        var personAgeTypeUpdatedAt = await queryable.ToListAsync(cancellationToken);

        return personAgeTypeUpdatedAt;
    }
}
