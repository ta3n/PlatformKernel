using Liberty.Cache.Services;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.OptionItemInventory;

public class OptionItemInventoryAdjustCommandHandler(
    ILogger<OptionItemInventoryAdjustCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOptionItemService optionItemService,
    IAppDateService appDateService,
    IOptionItemAppDateService optionItemAppDateService,
    IBookingRoomAppDateService bookingRoomAppDateService,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor
) : UpdateCommandHandlerBase<OptionItemInventoryAdjustCommand, long[]>(unitOfWork, mapper)
{
    private const int MaxPayloadCount = 1000;

    protected override async Task<long[]> HandleAsync(
        OptionItemInventoryAdjustCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload.ToList();

        if (payload.Count > MaxPayloadCount)
        {
            throw new OptionItemInventoryMaximumPayloadException(MaxPayloadCount);
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

        try
        {
            var notCreatedAppDateIds = await appDateService.FindNotCreatedAppDatesAsync(
                appDateIds,
                cancellationToken
            );

            if (notCreatedAppDateIds is { Count: > 0 })
            {
                await bookingRoomAppDateService.BulkUpsertAppDateAsync(
                    notCreatedAppDateIds
                );
            }

            var appDatesToUpsert = BuildAppDatesOfOptionItems(
                payload,
                appDatesOfOptionItems
            );

            await optionItemAppDateService.BulkUpsertOptionItemAppDateAsync(appDatesToUpsert);

            var optionItemEditIds = appDatesToUpsert
                .Select(x => x.OptionItemId)
                .Distinct()
                .ToArray();

            foreach (var optionItemEditId in optionItemEditIds)
            {
                await optionItemService.UpdateLastModifiedAsync(optionItemEditId, cancellationToken);
            }

            await cacheService.RemoveByPatternsAsync(
                true,
                $"*{string.Format(CacheKeys.FacilityPrefixKey, securityContextAccessor.FacilityKey)}*"
            );

            return [.. appDatesToUpsert.Select(x => x.AppDateId).Distinct()];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Adjust app dates of option items failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private static List<OptionItemAppDate> BuildAppDatesOfOptionItems(
        List<OptionItemChangeRemainRequest> payload,
        List<OptionItemAppDate> appDatesOfOptionItems
    )
    {
        var result = new List<OptionItemAppDate>();

        foreach (var appDate in payload)
        {
            var existingAppDateOfOptionItem = appDatesOfOptionItems.Find(
                x => x.OptionItemId == appDate.OptionItemId && x.AppDateId == appDate.AppDateId
            );

            OptionItemAppDate upsertAppDate;

            if (existingAppDateOfOptionItem is null)
            {
                upsertAppDate = new OptionItemAppDate
                {
                    OptionItemId = appDate.OptionItemId,
                    SellNumber = appDate.SellNumber,
                    IsNotSelled = appDate.IsNotSold,
                    AppDateId = appDate.AppDateId
                };
            }
            else
            {
                upsertAppDate = existingAppDateOfOptionItem.Clone<OptionItemAppDate>();
                upsertAppDate.SellNumber = appDate.SellNumber;
                upsertAppDate.IsNotSelled = appDate.IsNotSold;
            }

            result.Add(upsertAppDate);
        }

        return result;
    }
}
