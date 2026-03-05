using Liberty.Reservation.Manager.Application.Auth;
using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class FileService(
    ILogger<FileService> logger,
    ISecurityContextAccessor securityContextAccessor,
    IFileRepository fileRepository
) : BaseService<File>(logger, fileRepository, new ImageNotfoundException()), IFileService
{
    protected override IQueryable<File> GetQueryable()
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return base.GetQueryable()
            .Where(
                x => x.FacilityFiles!.Any(
                    t => t.FacilityId == facilityId
                )
            );
    }
}
