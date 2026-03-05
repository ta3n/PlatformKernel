using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IFacilityFileService : IBaseServiceRelation<FacilityFile>
{
    Task<FacilityFile> GetByFileIdAsync(
        long fileId,
        CancellationToken cancellationToken = default
    );
}
