namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupInventory;

public class RoomGroupInventoryAdjustCommandHandler(
    ILogger<RoomGroupInventoryAdjustCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRoomGroupService roomGroupService,
    IAppDateService appDateService,
    IRoomGroupAppDateService roomGroupAppDateService
) : UpdateCommandHandlerBase<RoomGroupInventoryAdjustCommand, (long[], long[])>(unitOfWork, mapper)
{
    private const int MaxInventoryLimit = 999;

    protected override async Task<(long[], long[])> HandleAsync(
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

        var startDate = appDateIds.Min();
        var endDate = appDateIds.Max();
        var existingAppDates = (await appDateService.FindAllByDateRangeAsync(
            AppDate.GetDateTime(startDate),
            AppDate.GetDateTime(endDate),
            cancellationToken
        )).ToList();

        var createAppDatesOfRoomGroups = CreateAppDatesOfRoomGroups(
            payload,
            appDatesOfRoomGroups,
            existingAppDates,
            out var updateAppDatesOfRoomGroups
        );

        var addAppDates = createAppDatesOfRoomGroups
            .Where(x => x.AppDate is not null)
            .Select(
                x => new
                {
                    x.AppDate!.Id,
                    x.AppDate!.DateTime
                }
            )
            .Distinct()
            .ToArray();
        createAppDatesOfRoomGroups.ForEach(x => x.AppDate = null);

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            await appDateService.CreateRangeAsync(
                addAppDates.Select(
                    x => new AppDate
                    {
                        Id = x.Id,
                        DateTime = x.DateTime,
                        IsEnabled = true
                    }
                ),
                false,
                cancellationToken
            );

            var addAppDatesOfRoom = await roomGroupAppDateService.CreateRangeAsync(
                createAppDatesOfRoomGroups,
                false,
                cancellationToken
            );

            var editAppDatesOfRoom = await roomGroupAppDateService.UpdateRangeAsync(
                updateAppDatesOfRoomGroups,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return (
                [.. addAppDatesOfRoom.Select(x => x.AppDateId).Distinct()],
                editAppDatesOfRoom.Select(x => x.AppDateId).Distinct().ToArray()
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Adjust app dates of room groups failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private static List<RoomGroupAppDate> CreateAppDatesOfRoomGroups(
        List<RoomGroupChangeRemainRequest> payload,
        List<RoomGroupAppDate> appDatesOfOptionItems,
        List<AppDate> existingAppDates,
        out List<RoomGroupAppDate> updateAppDatesOfRoomGroups
    )
    {
        var createAppDatesOfRoomGroups = new List<RoomGroupAppDate>();
        updateAppDatesOfRoomGroups = [];

        foreach (var appDate in payload)
        {
            var existingAppDateOfOptionItem = appDatesOfOptionItems.Find(
                x => x.RoomGroupId == appDate.RoomGroupId && x.AppDateId == appDate.AppDateId
            );
            var existingAppDate = existingAppDates.Find(
                x => x.DateTime == AppDate.GetDateTime(appDate.AppDateId)
            );
            if (existingAppDateOfOptionItem is null)
            {
                var addAppDate = new RoomGroupAppDate
                {
                    RoomGroupId = appDate.RoomGroupId,
                    SellNumber = appDate.SellNumber,
                    IsNotSelled = appDate.IsNotSold
                };

                if (existingAppDate is null)
                {
                    addAppDate.AppDateId = appDate.AppDateId;
                    addAppDate.AppDate = new()
                    {
                        Id = appDate.AppDateId,
                        DateTime = AppDate.GetDateTime(appDate.AppDateId)
                    };
                }
                else
                {
                    addAppDate.AppDateId = existingAppDate.Id;
                }

                createAppDatesOfRoomGroups.Add(addAppDate);
            }
            else
            {
                var editAppDate = existingAppDateOfOptionItem.Clone<RoomGroupAppDate>();
                editAppDate.SellNumber = appDate.SellNumber;
                editAppDate.IsNotSelled = appDate.IsNotSold;

                updateAppDatesOfRoomGroups.Add(editAppDate);
            }
        }

        return createAppDatesOfRoomGroups;
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
