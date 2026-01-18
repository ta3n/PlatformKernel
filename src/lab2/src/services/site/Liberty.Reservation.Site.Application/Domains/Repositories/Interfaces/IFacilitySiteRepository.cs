using Liberty.Reservation.Site.Application.Models;

namespace Liberty.Reservation.Site.Application.Domains.Repositories.Interfaces;

public interface IFacilitySiteRepository : IRepositoryBase<FacilitySite>
{
    Task<FacilitySiteModel?> GetSiteCodeAlreadyInFacilityAsync(
        string facilityCode,
        string siteCode,
        CancellationToken cancellationToken = default
    );
}
