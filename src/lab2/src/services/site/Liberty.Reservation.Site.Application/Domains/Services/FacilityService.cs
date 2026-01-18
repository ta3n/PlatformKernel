using Liberty.Reservation.Site.Application.Exceptions;

namespace Liberty.Reservation.Site.Application.Domains.Services;

public class FacilityService(
    ILogger<FacilityService> logger,
    IFacilityRepository facilityRepository,
    IFacilitySiteRepository facilitySiteRepository
) : BaseService<Facility>(logger, facilityRepository, new FacilityNotfoundException()), IFacilityService
{
    public async Task<(bool isAvailable, long facilityId, long siteId)> CheckSiteCodeAlreadyInFacilityAsync(
        string facilityCode,
        string siteCode,
        CancellationToken cancellationToken = default
    )
    {
        var siteExistingOfFacility = await facilitySiteRepository.GetSiteCodeAlreadyInFacilityAsync(
            facilityCode,
            siteCode,
            cancellationToken
        );

        return siteExistingOfFacility is null
            ? (false, 0, 0)
            : (true, siteExistingOfFacility.FacilityId, siteExistingOfFacility.SiteId);
    }

    public async Task<(string? fax, string? mail)> GetSystemMailAddressAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == facilityId)
            .Where(x => x.IsEnabled)
            .Select(
                x =>
                    new
                    {
                        x.Fax,
                        x.UseFax,
                        x.Meta!.SystemEMail
                    }
            );

        var facility = await queryable.SingleOrDefaultAsync(cancellationToken);

        var fax = facility?.UseFax is true ? facility.Fax : null;
        var mail = facility?.SystemEMail?.Trim();

        return (fax, mail);
    }
}
