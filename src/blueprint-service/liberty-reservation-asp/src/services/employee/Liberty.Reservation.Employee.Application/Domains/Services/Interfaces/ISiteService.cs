using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface ISiteService : IBaseService<Site>
{
    Task<bool> CheckExistingCode(
        string code,
        IEnumerable<long> ignoreIds,
        CancellationToken cancellationToken = default
    );
}
