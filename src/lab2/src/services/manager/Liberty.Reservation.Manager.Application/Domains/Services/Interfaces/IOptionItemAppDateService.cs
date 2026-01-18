using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IOptionItemAppDateService : IBaseServiceRelation<OptionItemAppDate>
{
    Task<IEnumerable<OptionItemAppDate>> FindAllByOptionItemIdsAsync(
        IEnumerable<long> optionItemIds,
        IEnumerable<long> appDateIds,
        CancellationToken cancellationToken = default
    );

    Task BulkUpsertOptionItemAppDateAsync(
        List<OptionItemAppDate> optionItemAppDates
    );
}
