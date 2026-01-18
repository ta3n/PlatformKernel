namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class AppDateService(
    ILogger<AppDateService> logger,
    IAppDateRepository appDateRepository
) : BaseService<AppDate>(logger, appDateRepository, new AppDateNotfoundException()), IAppDateService
{
    public async Task<IEnumerable<AppDate>> FindAllByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = appDateRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.DateTime >= startDate && x.DateTime <= endDate);

        var data = await queryable.ToListAsync(cancellationToken);

        return data;
    }

    public async Task<List<long>> FindNotCreatedAppDatesAsync(
        IEnumerable<long> appDateIds,
        CancellationToken cancellationToken = default
    )
    {
        var appDateIdList = appDateIds.Distinct().ToList();

        var existingDateTimes = await appDateRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => appDateIdList.Contains(x.Id))
            .Select(x => x.DateTime)
            .ToListAsync(cancellationToken);

        var existingSet = new HashSet<DateTime>(existingDateTimes);

        var notCreatedIds = appDateIdList
            .Where(id => !existingSet.Contains(AppDate.GetDateTime(id)))
            .ToList();

        return notCreatedIds;
    }
}
