using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Application.Domains.Repositories.Interfaces;

public interface IBookingPersonAgeTypeRepository : IRepositoryBase<PersonAgeType>
{
    Task<int> CountByIdsAsync(
        long facilityId,
        long[] ids,
        CancellationToken cancellationToken = default
    );
}
