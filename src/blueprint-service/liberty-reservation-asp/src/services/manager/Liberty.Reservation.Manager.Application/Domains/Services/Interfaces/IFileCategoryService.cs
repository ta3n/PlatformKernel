using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IFileCategoryService : IBaseServiceRelation<FileCategory>
{
    Task<(IEnumerable<FileCategory> adds, IEnumerable<FileCategory> removes)> ChangeFileCategoryAsync(
        long fileId,
        List<long> fileCategoryIds,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );
}
