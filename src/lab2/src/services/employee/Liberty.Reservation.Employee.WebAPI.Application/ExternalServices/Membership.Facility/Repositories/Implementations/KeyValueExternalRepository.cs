using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.Pagination;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Models;

namespace Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Repositories.Implementations;

public class KeyValueExternalRepository(
    ILogger<FacilityExternalRepository> logger,
    MembershipFacilityExternalDbContext dbContext
)
    : GenericRepository<KeyValue>(dbContext),
        IKeyValueExternalRepository
{
    public async Task<IPage<KeyValue>?> GetAllKeyValuesByRecordAsync(
        string recordCode,
        IPageable pageable,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var queryable = GetQueryableWithAsNoTracking()
                .Where(x => x.Record == recordCode);

            return await queryable.UsePageableAsync(
                pageable,
                cancellationToken: cancellationToken
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(GetAllKeyValuesByRecordAsync), ex.Message);
            return new Page<KeyValue>([], pageable, 0);
        }
    }
}
