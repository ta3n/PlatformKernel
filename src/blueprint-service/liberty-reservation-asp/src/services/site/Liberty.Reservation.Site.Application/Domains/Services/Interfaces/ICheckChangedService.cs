using Liberty.Reservation.Site.Application.Models;

namespace Liberty.Reservation.Site.Application.Domains.Services.Interfaces;

public interface ICheckChangedService
{
    Task<LastUpdatedTimeOfBoookingModel> GetLastUpdatedAtAsync(
        long planId,
        long roomId,
        CancellationToken cancellationToken
    );
}
