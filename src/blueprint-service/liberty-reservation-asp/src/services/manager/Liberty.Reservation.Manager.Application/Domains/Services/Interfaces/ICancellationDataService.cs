using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface ICancellationDataService : IBaseService<CancellationData>
{
    Task<(IEnumerable<CancellationData> entitiesToCreate, IEnumerable<long> idsToDelete)> AdjustRangeAsync(
        IEnumerable<CancellationData> entitiesToEdit,
        IEnumerable<CancellationCancellationData> existingDataOfCancellation,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );
}
