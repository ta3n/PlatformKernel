using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IFileRoomGroupService : IBaseServiceRelation<FileRoomGroup>
{
    Task<IEnumerable<FileRoomGroup>> FindAllByRoomGroupIdAsync(
        long roomGroupId,
        CancellationToken cancellationToken = default
    );

    Task<(IEnumerable<FileRoomGroup> adds, IEnumerable<FileRoomGroup> removes)>
        ChangeFilesOfRoomGroupAsync(
            long roomGroupId,
            IEnumerable<(long FileId, int Index)> iamges,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        );

    Task<List<FileRoomGroup>> GetByFileIdAsync(
        long fileId,
        CancellationToken cancellationToken = default
    );
}
