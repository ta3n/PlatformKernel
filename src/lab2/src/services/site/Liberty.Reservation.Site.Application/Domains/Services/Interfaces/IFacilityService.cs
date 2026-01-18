namespace Liberty.Reservation.Site.Application.Domains.Services.Interfaces;

public interface IFacilityService : IBaseService<Facility>
{
    Task<(bool isAvailable, long facilityId, long siteId)> CheckSiteCodeAlreadyInFacilityAsync(
        string facilityCode,
        string siteCode,
        CancellationToken cancellationToken = default
    );

    Task<(string? fax, string? mail)> GetSystemMailAddressAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    );
}
