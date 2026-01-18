namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class FacilityFileService(
    ILogger<FacilityFileService> logger,
    IFacilityFileRepository repository
) : BaseServiceRelation<FacilityFile>(logger, repository), IFacilityFileService
{
    public async Task<FacilityFile> GetByFileIdAsync(
        long fileId,
        CancellationToken cancellationToken = default
    )
    {
        var data = await repository
                .GetQueryableWithAsNoTracking()
                .Include(x => x.File)
                .FirstOrDefaultAsync(
                    x => x.FileId == fileId,
                    cancellationToken
                )
            ?? throw new FacilityFileNotFoundException();

        return data;
    }
}
