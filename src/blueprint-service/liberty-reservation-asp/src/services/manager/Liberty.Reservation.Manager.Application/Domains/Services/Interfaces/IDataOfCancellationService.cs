using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IDataOfCancellationService : IBaseServiceRelation<CancellationCancellationData>
{
    Task<IEnumerable<CancellationCancellationData>> FindAllByCancellationIdAsync(
        long cancellationId,
        CancellationToken cancellationToken = default
    );

    Task<List<CancellationData>> GetAllCancellationDataAsync(
        long cancellationId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<CancellationCancellationData>> CreateRangeAsync(
        long cancellationId,
        IEnumerable<CancellationData> newCancellationData,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<CancellationCancellationData>> DeleteRangeAsync(
        long cancellationId,
        IEnumerable<long> cancellationDataIds,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );
}
