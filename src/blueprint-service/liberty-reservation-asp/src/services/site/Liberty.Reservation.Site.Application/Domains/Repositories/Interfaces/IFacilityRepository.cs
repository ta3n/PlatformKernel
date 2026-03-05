namespace Liberty.Reservation.Site.Application.Domains.Repositories.Interfaces;

public interface IFacilityRepository : IRepositoryBase<Facility>
{
    Task<(bool isAvailable, long facilityId)> CheckFacilityAvailableAsync(
        string facilityCode,
        CancellationToken cancellationToken = default
    );

    Task<long?> GetFacilityUpdatedTimeAvailableAsync(
        long? facilityId,
        DbContext? appDbContext,
        CancellationToken cancellationToken = default
    );
}
