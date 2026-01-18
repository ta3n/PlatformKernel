namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class DataOfCancellationService(
    ILogger<DataOfCancellationService> logger,
    IDataOfCancellationRepository dataOfCancellationRepository
) : BaseServiceRelation<CancellationCancellationData>(logger, dataOfCancellationRepository),
    IDataOfCancellationService
{
    public async Task<IEnumerable<CancellationCancellationData>> FindAllByCancellationIdAsync(
        long cancellationId,
        CancellationToken cancellationToken = default
    )
    {
        var response = await dataOfCancellationRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.CancellationId == cancellationId)
            .ToListAsync(cancellationToken);

        return response;
    }

    public async Task<List<CancellationData>> GetAllCancellationDataAsync(
        long cancellationId,
        CancellationToken cancellationToken = default
    )
    {
        var query = dataOfCancellationRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.CancellationId == cancellationId)
            .Select(x => x.CancellationData!)
            .OrderBy(x => x.Id);
        var cancellationDatas = await query.ToListAsync(cancellationToken) ?? throw new CancellationDataNotFoundException();
        return cancellationDatas;
    }

    public async Task<IEnumerable<CancellationCancellationData>> CreateRangeAsync(
        long cancellationId,
        IEnumerable<CancellationData> newCancellationData,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var entitiesToCreate = newCancellationData
            .Select(
                x => new CancellationCancellationData
                {
                    CancellationId = cancellationId,
                    CancellationData = x
                }
            );

        var listDataOfCancellation = await CreateRangeAsync(
            entitiesToCreate,
            autoSave,
            cancellationToken
        );

        return listDataOfCancellation;
    }

    public async Task<IEnumerable<CancellationCancellationData>> DeleteRangeAsync(
        long cancellationId,
        IEnumerable<long> cancellationDataIds,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var entitiesToDelete = new List<CancellationCancellationData>();
        foreach (var id in cancellationDataIds)
        {
            entitiesToDelete.Add(
                new CancellationCancellationData
                {
                    CancellationId = cancellationId,
                    CancellationDataId = id
                }
            );
        }

        var listDataOfCancellation = await DeleteRangeAsync(
            entitiesToDelete,
            autoSave,
            cancellationToken
        );

        return listDataOfCancellation;
    }
}
