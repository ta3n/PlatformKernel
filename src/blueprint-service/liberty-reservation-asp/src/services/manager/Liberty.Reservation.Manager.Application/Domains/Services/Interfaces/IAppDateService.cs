using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IAppDateService : IBaseService<AppDate>
{
    Task<IEnumerable<AppDate>> FindAllByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default
    );

    Task<List<long>> FindNotCreatedAppDatesAsync(
        IEnumerable<long> appDateIds,
        CancellationToken cancellationToken = default
    );
}
