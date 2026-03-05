using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class OptionItemCategoryService(
    ILogger<OptionItemCategoryService> logger,
    IOptionItemCategoryRepository optionItemCategoryRepository
) : BaseServiceRelation<OptionItemCategory>(logger, optionItemCategoryRepository), IOptionItemCategoryService
{
    public async Task<IEnumerable<OptionItemCategory>> FindAllByOptionItemIdAsync(
        long optionItemId,
        CancellationToken cancellationToken = default
    )
    {
        var data = await optionItemCategoryRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.OptionItemId == optionItemId)
            .ToListAsync(cancellationToken);

        return data;
    }

    public async Task<(IEnumerable<OptionItemCategory> adds, IEnumerable<OptionItemCategory> removes)>
        ChangeCategoriesOfOptionItemAsync(
            long optionItemId,
            IEnumerable<long> categoryIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var existingCategoriesOfRoomGroup = (await FindAllByOptionItemIdAsync(
            optionItemId,
            cancellationToken
        )).ToList();
        var updateCategoriesOfOptionItem = categoryIds
            .Select(
                categoryId => new OptionItemCategory
                {
                    OptionItemId = optionItemId,
                    CategoryId = categoryId
                }
            )
            .ToList();

        var comparer = new DelegateEqualityComparer<OptionItemCategory>(
            (
                x,
                y
            ) => x?.OptionItemId == y?.OptionItemId && x?.CategoryId == y?.CategoryId,
            obj => obj.OptionItemId.GetHashCode() ^ obj.CategoryId.GetHashCode()
        );

        var removeCategoriesInOptionItem = existingCategoriesOfRoomGroup.Except(
            updateCategoriesOfOptionItem,
            comparer
        );
        var addCategoriesInOptionItem = updateCategoriesOfOptionItem.Except(
            existingCategoriesOfRoomGroup,
            comparer
        );

        var removes = await DeleteRangeAsync(
            removeCategoriesInOptionItem,
            autoSave,
            cancellationToken
        );

        var adds = await CreateRangeAsync(
            addCategoriesInOptionItem,
            autoSave,
            cancellationToken
        );

        return (adds, removes);
    }
}
