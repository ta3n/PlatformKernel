namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class OptionItemAppDateService(
    ILogger<OptionItemAppDateService> logger,
    IOptionItemAppDateRepository optionItemAppDateRepository
) : BaseServiceRelation<OptionItemAppDate>(logger, optionItemAppDateRepository), IOptionItemAppDateService
{
    public async Task<IEnumerable<OptionItemAppDate>> FindAllByOptionItemIdsAsync(
        IEnumerable<long> optionItemIds,
        IEnumerable<long> appDateIds,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = optionItemAppDateRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => optionItemIds.Contains(x.OptionItemId))
            .Where(x => appDateIds.Contains(x.AppDateId));

        var data = await queryable.ToListAsync(cancellationToken);

        return data;
    }
}
