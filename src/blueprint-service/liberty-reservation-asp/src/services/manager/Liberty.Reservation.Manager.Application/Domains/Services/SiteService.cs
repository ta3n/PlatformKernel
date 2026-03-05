namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class SiteService(
    ILogger<SiteService> logger,
    ISiteRepository siteRepository
) : BaseService<Site>(
        logger,
        siteRepository,
        new SiteNotfoundException()
    ),
    ISiteService
{
    public async Task<long> GetSiteIdBySiteCodeAsync(
        string? siteCode,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = siteRepository.GetQueryableWithAsNoTracking()
            .Where(x => x.Code == siteCode)
            .Select(x => x.Id);

        var siteId = await queryable.SingleOrDefaultAsync(cancellationToken);

        return siteId;
    }
}
