using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class FileOptionItemService(
    ILogger<FileOptionItemService> logger,
    IFileOptionItemRepository fileOptionItemRepository
) : BaseServiceRelation<FileOptionItem>(logger, fileOptionItemRepository), IFileOptionItemService
{
    public async Task<IEnumerable<FileOptionItem>> FindAllByOptionItemIdAsync(
        long optionItemId,
        CancellationToken cancellationToken = default
    )
    {
        var data = await fileOptionItemRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.OptionItemId == optionItemId)
            .ToListAsync(cancellationToken);

        return data;
    }

    public async Task<(IEnumerable<FileOptionItem> adds, IEnumerable<FileOptionItem> removes)>
        ChangeFilesOfOptionItemAsync(
            long optionItemId,
            IEnumerable<(long FileId, int Index)> fileIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var existingImagesOfOptionItem = (await FindAllByOptionItemIdAsync(
            optionItemId,
            cancellationToken
        )).ToList();
        var updateImagesOfOptionItem = fileIds
            .Select(
                image => new FileOptionItem
                {
                    OptionItemId = optionItemId,
                    FileId = image.FileId,
                    Index = image.Index
                }
            )
            .ToList();

        var comparer = new DelegateEqualityComparer<FileOptionItem>(
            (
                x,
                y
            ) => x?.FileId == y?.FileId && x?.Index == y?.Index && x?.OptionItemId == y?.OptionItemId,
            obj => obj.FileId.GetHashCode() ^ obj.OptionItemId.GetHashCode() ^ obj.Index.GetHashCode()
        );

        var removeImagesInOptionItem = existingImagesOfOptionItem.Except(
            updateImagesOfOptionItem,
            comparer
        );
        var addImagesInOptionItem = updateImagesOfOptionItem.Except(
            existingImagesOfOptionItem,
            comparer
        );

        var removes = await DeleteRangeAsync(
            removeImagesInOptionItem,
            autoSave,
            cancellationToken
        );

        var adds = await CreateRangeAsync(
            addImagesInOptionItem,
            autoSave,
            cancellationToken
        );

        return (adds, removes);
    }

    public async Task<List<FileOptionItem>> GetByFileIdAsync(
        long fileId,
        CancellationToken cancellationToken = default
    )
    {
        var data = await fileOptionItemRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.FileId == fileId
            )
            .ToListAsync(cancellationToken);

        return data;
    }
}
