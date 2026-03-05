using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IOptionItemCategoryService : IBaseServiceRelation<OptionItemCategory>
{
    Task<IEnumerable<OptionItemCategory>> FindAllByOptionItemIdAsync(
        long optionItemId,
        CancellationToken cancellationToken = default
    );

    Task<(IEnumerable<OptionItemCategory> adds, IEnumerable<OptionItemCategory> removes)>
        ChangeCategoriesOfOptionItemAsync(
            long optionItemId,
            IEnumerable<long> categoryIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        );
}
