using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IOptionItemService : IBaseService<OptionItem>
{
    Task<long> CountAvailableByIdsAsync(
        long[] ids,
        DateTime dateCheck,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<OptionItemAppDate>> FindAllAvailableAppDatesOfOptionItemsAsync(
        long[] optionItemIds,
        long startAppDateCheckId,
        long endAppDateCheckId,
        CancellationToken cancellationToken = default
    );

    Task UpdateLastModifiedAsync(
        long id,
        CancellationToken cancellationToken = default
    );
}
