using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class FileRoomGroupService(
    ILogger<FileRoomGroupService> logger,
    IFileRoomGroupRepository fileRoomGroupRepository
) : BaseServiceRelation<FileRoomGroup>(logger, fileRoomGroupRepository), IFileRoomGroupService
{
    public async Task<IEnumerable<FileRoomGroup>> FindAllByRoomGroupIdAsync(
        long roomGroupId,
        CancellationToken cancellationToken = default
    )
    {
        var data = await fileRoomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.RoomGroupId == roomGroupId)
            .ToListAsync(cancellationToken);

        return data;
    }

    public async Task<(IEnumerable<FileRoomGroup> adds, IEnumerable<FileRoomGroup> removes)>
        ChangeFilesOfRoomGroupAsync(
            long roomGroupId,
            IEnumerable<(long FileId, int Index)> iamges,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var existingFilesOfRoomGroup = (await FindAllByRoomGroupIdAsync(
            roomGroupId,
            cancellationToken
        )).ToList();
        var updateFilesOfRoomGroup = iamges
            .Select(
                image => new FileRoomGroup
                {
                    RoomGroupId = roomGroupId,
                    FileId = image.FileId,
                    Index = image.Index
                }
            )
            .ToList();

        var comparer = new DelegateEqualityComparer<FileRoomGroup>(
            (
                x,
                y
            ) => x?.FileId == y?.FileId && x?.Index == y?.Index && x?.RoomGroupId == y?.RoomGroupId,
            obj => obj.FileId.GetHashCode() ^ obj.RoomGroupId.GetHashCode() ^ obj.Index.GetHashCode()
        );

        IEnumerable<FileRoomGroup> addImagesInRoomGroup = updateFilesOfRoomGroup;
        IEnumerable<FileRoomGroup> removes = [];

        if (existingFilesOfRoomGroup.Count > 0)
        {
            addImagesInRoomGroup = updateFilesOfRoomGroup.Except(existingFilesOfRoomGroup, comparer);

            var removeImagesInRoomGroup = existingFilesOfRoomGroup.Except(updateFilesOfRoomGroup, comparer);
            removes = await DeleteRangeAsync(
                removeImagesInRoomGroup,
                autoSave,
                cancellationToken
            );
        }

        var adds = await CreateRangeAsync(
            addImagesInRoomGroup,
            autoSave,
            cancellationToken
        );

        return (adds, removes);
    }

    public async Task<List<FileRoomGroup>> GetByFileIdAsync(
        long fileId,
        CancellationToken cancellationToken = default
    )
    {
        var data = await fileRoomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.FileId == fileId
            )
            .ToListAsync(cancellationToken);

        return data;
    }
}
