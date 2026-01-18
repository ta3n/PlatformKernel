namespace Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;

public interface IFacilityRepository : IRepositoryBase<Facility>
{
    Task<(bool isAvailable, long facilityId, TimeSpan facilityTimeZone)> CheckFacilityAvailableAsync(
        string facilityCode,
        CancellationToken cancellationToken = default
    );
}
