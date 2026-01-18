namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class FacilityOptionItemService(
    ILogger<FacilityOptionItemService> logger,
    IFacilityOptionItemRepository facilityOptionItemRepository
) : BaseServiceRelation<FacilityOptionItem>(logger, facilityOptionItemRepository), IFacilityOptionItemService
{
    public async Task<IEnumerable<FacilityOptionItem>> FindAllByOptionItemIdAsync(
        long optionItemId,
        CancellationToken cancellationToken = default
    )
    {
        var data = await facilityOptionItemRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.OptionItemId == optionItemId)
            .ToListAsync(cancellationToken);

        return data;
    }

    public async Task<IEnumerable<FacilityOptionItem>> FindAllByFacilityIdAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        var data = await facilityOptionItemRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.FacilityId == facilityId)
            .ToListAsync(cancellationToken);

        return data;
    }
}
