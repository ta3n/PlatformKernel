using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Application.Domains.Repositories.Interfaces;

public interface IBookingOptionItemRepository : IRepositoryBase<OptionItem>
{
    Task<long> CountAvailableByIdsAsync(
        long facilityId,
        long[] ids,
        DateTime dateCheck,
        CancellationToken cancellationToken = default
    );
}
