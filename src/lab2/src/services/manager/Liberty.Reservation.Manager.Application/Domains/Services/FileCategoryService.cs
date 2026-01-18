using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class FileCategoryService(
    ILogger<FileCategoryService> logger,
    IFileCategoryRepository repository
) : BaseServiceRelation<FileCategory>(logger, repository), IFileCategoryService
{
    public async Task<(IEnumerable<FileCategory> adds, IEnumerable<FileCategory> removes)>
        ChangeFileCategoryAsync(
            long fileId,
            List<long> fileCategoryIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var existingFileCategory = (await GetAllFileCategoryByFileIdAsync(
            fileId,
            cancellationToken
        )).ToList();

        var updateFileCategory = fileCategoryIds
            .Select(
                categoryId => new FileCategory
                {
                    FileId = fileId,
                    CategoryId = categoryId
                }
            )
            .ToList();

        var comparer = new DelegateEqualityComparer<FileCategory>(
            (
                x,
                y
            ) => x?.FileId == y?.FileId && x!.CategoryId == y?.CategoryId,
            obj => obj.FileId.GetHashCode() ^ obj.CategoryId.GetHashCode()
        );

        var addFileCategory = updateFileCategory.Except(
            existingFileCategory,
            comparer
        );

        var adds = await CreateRangeAsync(
            addFileCategory,
            autoSave,
            cancellationToken
        );

        IEnumerable<FileCategory> removes = [];
        if (existingFileCategory.Count > 0)
        {
            var removeFileCategory = existingFileCategory.Except(
                updateFileCategory,
                comparer
            );
            removes = await DeleteRangeAsync(
                removeFileCategory,
                autoSave,
                cancellationToken
            );
        }

        return (adds, removes);
    }

    private async Task<List<FileCategory>> GetAllFileCategoryByFileIdAsync(
        long fileId,
        CancellationToken cancellationToken = default
    )
    {
        return await repository.GetQueryableWithAsNoTracking()
            .Where(x => x.FileId == fileId)
            .ToListAsync(cancellationToken);
    }
}
