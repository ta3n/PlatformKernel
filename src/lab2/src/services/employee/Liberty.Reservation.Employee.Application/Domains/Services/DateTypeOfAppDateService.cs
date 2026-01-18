using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class DateTypeOfAppDateService(
    ILogger<DateTypeOfAppDateService> logger,
    IDateTypeOfAppDateRepository dateTypeOfAppDateRepository
) : BaseServiceRelation<AppDateAppDateType>(logger, dateTypeOfAppDateRepository), IDateTypeOfAppDateService
{
    public async Task<AppDateAppDateType?> FindByAppDateIdAsync(
        long appDateId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = dateTypeOfAppDateRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.AppDateId == appDateId);

        var data = await queryable.SingleOrDefaultAsync(
            cancellationToken
        );

        return data;
    }

    public async Task<IEnumerable<AppDateAppDateType>> FindAllByAppDateAsync(
        long appDate,
        CancellationToken cancellationToken = default
    )
    {
        var date = AppDate.GetDateTime(appDate);

        var queryable = dateTypeOfAppDateRepository
            .GetQueryable()
            .IgnoreQueryFilters()
            .Include(x => x.AppDate)
            .Include(x => x.AppDateType)
            .Where(x => x.AppDate!.DateTime == date);

        var data = await queryable.ToListAsync(
            cancellationToken
        );

        return data;
    }
}
