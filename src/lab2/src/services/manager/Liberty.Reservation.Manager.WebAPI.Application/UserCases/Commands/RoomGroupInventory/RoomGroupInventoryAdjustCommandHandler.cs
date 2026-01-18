using Liberty.Cache.Services;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupInventory;

public class RoomGroupInventoryAdjustCommandHandler(
    ILogger<RoomGroupInventoryAdjustCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRoomGroupService roomGroupService,
    IAppDateService appDateService,
    IRoomGroupAppDateService roomGroupAppDateService,
    IBookingRoomAppDateService bookingRoomAppDateService,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor
) : UpdateCommandHandlerBase<RoomGroupInventoryAdjustCommand, long[]>(unitOfWork, mapper)
{
    private const int MaxInventoryLimit = 999;

    protected override async Task<long[]> HandleAsync(
        RoomGroupInventoryAdjustCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload.ToList();

        var roomGroupIds = payload
            .Select(x => x.RoomGroupId)
            .Distinct()
            .ToArray();

        await ValidateRoomGroup(payload, roomGroupIds, cancellationToken);

        var appDateIds = payload
            .Select(x => x.AppDateId)
            .Distinct()
            .ToArray();

        var appDatesOfRoomGroups = (await roomGroupAppDateService.FindAllByRoomGroupIdsAsync(
            roomGroupIds,
            appDateIds,
            cancellationToken
        )).ToList();

        var notCreatedAppDateIds = await appDateService.FindNotCreatedAppDatesAsync(
            [.. appDateIds],
            cancellationToken
        );

        try
        {
            if (notCreatedAppDateIds is { Count: > 0 })
            {
                await bookingRoomAppDateService.BulkUpsertAppDateAsync(
                    notCreatedAppDateIds
                );
            }

            var startDate = appDateIds.Min();
            var endDate = appDateIds.Max();

            var existingAppDates = (await appDateService.FindAllByDateRangeAsync(
                AppDate.GetDateTime(startDate),
                AppDate.GetDateTime(endDate),
                cancellationToken
            )).ToList();

            var appDatesToUpsert = BuildAppDatesOfRoomGroups(
                payload,
                appDatesOfRoomGroups,
                existingAppDates
            );

            await bookingRoomAppDateService.BulkUpsertRoomGroupAppDateAsync(appDatesToUpsert);

            await cacheService.RemoveByPatternsAsync(
                true,
                $"*{string.Format(CacheKeys.FacilityPrefixKey, securityContextAccessor.FacilityKey)}*"
            );

            return [.. appDatesToUpsert.Select(x => x.AppDateId).Distinct()];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Adjust app dates of room groups failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private static List<RoomGroupAppDate> BuildAppDatesOfRoomGroups(
        List<RoomGroupChangeRemainRequest> payload,
        List<RoomGroupAppDate> appDatesOfOptionItems,
        List<AppDate> existingAppDates
    )
    {
        var result = new List<RoomGroupAppDate>();

        foreach (var appDate in payload)
        {
            var existingAppDateOfOptionItem = appDatesOfOptionItems.Find(
                x => x.RoomGroupId == appDate.RoomGroupId && x.AppDateId == appDate.AppDateId
            );

            var existingAppDate = existingAppDates.Find(
                x => x.DateTime == AppDate.GetDateTime(appDate.AppDateId)
            );

            RoomGroupAppDate upsertAppDate;

            if (existingAppDateOfOptionItem is null)
            {
                upsertAppDate = new RoomGroupAppDate
                {
                    RoomGroupId = appDate.RoomGroupId,
                    SellNumber = appDate.SellNumber,
                    IsNotSelled = appDate.IsNotSold
                };

                if (existingAppDate is null)
                {
                    upsertAppDate.AppDateId = appDate.AppDateId;
                    upsertAppDate.AppDate = new AppDate
                    {
                        Id = appDate.AppDateId,
                        DateTime = AppDate.GetDateTime(appDate.AppDateId)
                    };
                }
                else
                {
                    upsertAppDate.AppDateId = existingAppDate.Id;
                }
            }
            else
            {
                upsertAppDate = existingAppDateOfOptionItem.Clone<RoomGroupAppDate>();
                upsertAppDate.SellNumber = appDate.SellNumber;
                upsertAppDate.IsNotSelled = appDate.IsNotSold;
            }

            result.Add(upsertAppDate);
        }

        return result;
    }

    private async Task ValidateRoomGroup(
        List<RoomGroupChangeRemainRequest> payload,
        long[] roomGroupIds,
        CancellationToken cancellationToken
    )
    {
        var existingCountRoomGroups = await roomGroupService.CountByIdsAsync(
            roomGroupIds,
            cancellationToken
        );
        if (existingCountRoomGroups != roomGroupIds.Length)
        {
            throw new RoomGroupNotfoundException();
        }

        var roomGroups = await roomGroupService.FindAllByIdsAsync(roomGroupIds, cancellationToken);

        var roomGroupInvalids = roomGroups
            .Where(x => payload.Exists(y => x.Id == y.RoomGroupId && y.SellNumber > MaxInventoryLimit))
            .ToList();

        if (roomGroupInvalids.Count > 0)
        {
            var roomGroupInvalid = roomGroupInvalids.FirstOrDefault();
            throw new RoomGroupInventoryInvalidException(roomGroupInvalid!.Id, MaxInventoryLimit);
        }
    }
}
