using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.Pagination;
using Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Models;

namespace Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;

public interface IKeyValueExternalRepository : IGenericRepository<KeyValue>
{
    Task<IPage<KeyValue>?> GetAllKeyValuesByRecordAsync(
        string recordCode,
        IPageable pageable,
        CancellationToken cancellationToken = default
    );
}
