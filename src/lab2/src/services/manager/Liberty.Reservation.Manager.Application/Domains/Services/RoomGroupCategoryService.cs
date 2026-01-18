using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class RoomGroupCategoryService(
    ILogger<RoomGroupCategoryService> logger,
    IRoomGroupCategoryRepository roomGroupCategoryRepository
) : BaseServiceRelation<RoomGroupCategory>(logger, roomGroupCategoryRepository), IRoomGroupCategoryService
{
    public async Task<IEnumerable<RoomGroupCategory>> FindAllByRoomGroupIdAsync(
        long roomGroupId,
        CancellationToken cancellationToken = default
    )
    {
        var data = await roomGroupCategoryRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.RoomGroupId == roomGroupId)
            .ToListAsync(cancellationToken);

        return data;
    }

    public async Task<(IEnumerable<RoomGroupCategory> adds, IEnumerable<RoomGroupCategory> removes)>
        ChangeCategoriesOfRoomGroupAsync(
            long roomGroupId,
            IEnumerable<long> categoryIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var existingCategoriesOfRoomGroup = (await FindAllByRoomGroupIdAsync(
            roomGroupId,
            cancellationToken
        )).ToList();
        var updateCategoriesOfRoomGroup = categoryIds
            .Select(
                categoryId => new RoomGroupCategory
                {
                    RoomGroupId = roomGroupId,
                    CategoryId = categoryId
                }
            )
            .ToList();

        var comparer = new DelegateEqualityComparer<RoomGroupCategory>(
            (
                x,
                y
            ) => x?.RoomGroupId == y?.RoomGroupId && x?.CategoryId == y?.CategoryId,
            obj => obj.RoomGroupId.GetHashCode() ^ obj.CategoryId.GetHashCode()
        );

        var removeCategoriesInRoomGroup = existingCategoriesOfRoomGroup.Except(
            updateCategoriesOfRoomGroup,
            comparer
        );
        var addCategoriesInRoomGroup = updateCategoriesOfRoomGroup.Except(
            existingCategoriesOfRoomGroup,
            comparer
        );

        var removes = await DeleteRangeAsync(
            removeCategoriesInRoomGroup,
            autoSave,
            cancellationToken
        );

        var adds = await CreateRangeAsync(
            addCategoriesInRoomGroup,
            autoSave,
            cancellationToken
        );

        return (adds, removes);
    }
}
