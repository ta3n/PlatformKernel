namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.OptionItemInventory;

public class OptionItemInventoryAdjustCommandHandler(
    ILogger<OptionItemInventoryAdjustCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOptionItemService optionItemService,
    IAppDateService appDateService,
    IOptionItemAppDateService optionItemAppDateService
) : UpdateCommandHandlerBase<OptionItemInventoryAdjustCommand, (long[], long[])>(unitOfWork, mapper)
{
    protected override async Task<(long[], long[])> HandleAsync(
        OptionItemInventoryAdjustCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload.ToList();

        if (payload.Count > 100)
        {
            throw new OptionItemInventoryMaximumPayloadException();
        }

        var optionItemIds = payload
            .Select(x => x.OptionItemId)
            .Distinct()
            .ToArray();
        var existingCountOptionItem = await optionItemService.CountByIdsAsync(
            optionItemIds,
            cancellationToken
        );
        if (existingCountOptionItem != optionItemIds.Length)
        {
            throw new OptionItemNotfoundException();
        }

        var appDateIds = payload
            .Select(x => x.AppDateId)
            .Distinct()
            .ToArray();
        var appDatesOfOptionItems = (await optionItemAppDateService.FindAllByOptionItemIdsAsync(
            optionItemIds,
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

        var createAppDatesOfOptionItems = CreateAppDatesOfOptionItems(
            payload,
            appDatesOfOptionItems,
            existingAppDates,
            out var updateAppDatesOfOptionItems
        );

        var addAppDates = createAppDatesOfOptionItems
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
        createAppDatesOfOptionItems.ForEach(x => x.AppDate = null);

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            foreach (var appdate in addAppDates)
            {
                var count = await appDateService.CountByIdsAsync(
                    [appdate.Id],
                    cancellationToken
                );
                if (count == 0)
                {
                    await appDateService.CreateAsync(
                        new AppDate
                        {
                            Id = appdate.Id,
                            DateTime = appdate.DateTime,
                            IsEnabled = true
                        },
                        false,
                        cancellationToken
                    );
                }
            }

            var addAppDatesOfOptionItem = await optionItemAppDateService.CreateRangeAsync(
                createAppDatesOfOptionItems,
                false,
                cancellationToken
            );

            var editAppDatesOfOptionItem = await optionItemAppDateService.UpdateRangeAsync(
                updateAppDatesOfOptionItems,
                false,
                cancellationToken
            );

            var optionItemEditIds = editAppDatesOfOptionItem
                .Select(x => x.OptionItemId)
                .Distinct()
                .ToArray();

            foreach (var optionItemEditId in optionItemEditIds)
            {
                await optionItemService.UpdateLastModifiedAsync(optionItemEditId, cancellationToken);
            }

            await UnitOfWork.CommitAsync(cancellationToken);

            return (
                [.. addAppDatesOfOptionItem.Select(x => x.AppDateId).Distinct()],
                editAppDatesOfOptionItem.Select(x => x.AppDateId).Distinct().ToArray()
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Adjust app dates of option items failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private static List<OptionItemAppDate> CreateAppDatesOfOptionItems(
        List<OptionItemChangeRemainRequest> payload,
        List<OptionItemAppDate> appDatesOfOptionItems,
        List<AppDate> existingAppDates,
        out List<OptionItemAppDate> updateAppDatesOfOptionItems
    )
    {
        var createAppDatesOfOptionItems = new List<OptionItemAppDate>();
        updateAppDatesOfOptionItems = [];

        foreach (var appDate in payload)
        {
            var existingAppDateOfOptionItem = appDatesOfOptionItems.Find(
                x => x.OptionItemId == appDate.OptionItemId && x.AppDateId == appDate.AppDateId
            );
            var existingAppDate = existingAppDates.Find(
                x => x.DateTime == AppDate.GetDateTime(appDate.AppDateId)
            );
            if (existingAppDateOfOptionItem is null)
            {
                var addAppDate = new OptionItemAppDate
                {
                    OptionItemId = appDate.OptionItemId,
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

                createAppDatesOfOptionItems.Add(addAppDate);
            }
            else
            {
                var editAppDate = existingAppDateOfOptionItem.Clone<OptionItemAppDate>();
                editAppDate.SellNumber = appDate.SellNumber;
                editAppDate.IsNotSelled = appDate.IsNotSold;

                updateAppDatesOfOptionItems.Add(editAppDate);
            }
        }

        return createAppDatesOfOptionItems;
    }
}
