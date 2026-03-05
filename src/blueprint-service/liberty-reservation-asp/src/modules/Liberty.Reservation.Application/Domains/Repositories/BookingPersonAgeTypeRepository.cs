using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Application.Domains.Repositories;

public class BookingPersonAgeTypeRepository(
    DbContext dataContext
) : RepositoryBase<PersonAgeType>(dataContext), IBookingPersonAgeTypeRepository
{
    public async Task<int> CountByIdsAsync(
        long facilityId,
        long[] ids,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = GetQueryableWithAsNoTracking()
            .Where(
                x => x.FacilityPersonAgeTypes!.Any(
                    t => t.FacilityId == facilityId
                )
            )
            .Where(
                x => ids.Contains(x.Id)
            );

        var existingCount = await queryable.CountAsync(
            cancellationToken
        );

        return existingCount;
    }
}
