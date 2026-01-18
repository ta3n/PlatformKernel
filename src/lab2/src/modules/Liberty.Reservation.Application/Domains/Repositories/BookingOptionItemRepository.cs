using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Application.Domains.Repositories;

public class BookingOptionItemRepository(
    DbContext dataContext
) : RepositoryBase<OptionItem>(dataContext), IBookingOptionItemRepository
{
    public async Task<long> CountAvailableByIdsAsync(
        long facilityId,
        long[] ids,
        DateTime dateCheck,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = GetQueryable()
            .Where(
                x => x.FacilityOptionItems!.Any(
                    t => t.FacilityId == facilityId
                )
            )
            .Where(x => ids.Contains(x.Id))
            .Where(x => x.IsEnabled)
            .Where(x => x.EnabledStart == null || x.EnabledStart <= dateCheck)
            .Where(x => x.EnabledEnd == null || x.EnabledEnd >= dateCheck)
            .Where(x => x.UseDisplayDate || x.DisplayDateStart == null || x.DisplayDateStart <= dateCheck)
            .Where(x => x.UseDisplayDate || x.DisplayDateEnd == null || x.DisplayDateEnd >= dateCheck)
            .Where(x => x.UseAcceptDate || x.AcceptDateStart == null || x.AcceptDateStart <= dateCheck)
            .Where(x => x.UseAcceptDate || x.AcceptDateEnd == null || x.AcceptDateEnd >= dateCheck);

        var existingCount = await queryable.CountAsync(
            cancellationToken
        );

        return existingCount;
    }
}
