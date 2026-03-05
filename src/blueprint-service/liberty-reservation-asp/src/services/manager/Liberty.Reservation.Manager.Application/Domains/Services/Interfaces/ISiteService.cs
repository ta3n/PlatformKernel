using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface ISiteService : IBaseService<Site>
{
    Task<long> GetSiteIdBySiteCodeAsync(
        string? siteCode,
        CancellationToken cancellationToken = default
    );
}
