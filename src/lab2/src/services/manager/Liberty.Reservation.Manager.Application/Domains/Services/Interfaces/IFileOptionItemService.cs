using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IFileOptionItemService : IBaseServiceRelation<FileOptionItem>
{
    Task<IEnumerable<FileOptionItem>> FindAllByOptionItemIdAsync(
        long optionItemId,
        CancellationToken cancellationToken = default
    );

    Task<(IEnumerable<FileOptionItem> adds, IEnumerable<FileOptionItem> removes)>
        ChangeFilesOfOptionItemAsync(
            long optionItemId,
            IEnumerable<(long FileId, int Index)> fileIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        );

    Task<List<FileOptionItem>> GetByFileIdAsync(
        long fileId,
        CancellationToken cancellationToken = default
    );
}
