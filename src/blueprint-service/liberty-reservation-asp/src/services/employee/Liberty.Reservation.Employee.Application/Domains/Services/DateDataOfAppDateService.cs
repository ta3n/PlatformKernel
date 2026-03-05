using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class DateDataOfAppDateService(
    ILogger<DateDataOfAppDateService> logger,
    IDateDataOfAppDateRepository dateDataOfAppDateRepository
) : BaseServiceRelation<AppDateAppDateData>(logger, dateDataOfAppDateRepository), IDateDataOfAppDateService
{
    public async Task<AppDateAppDateData?> FindByAppDateIdAsync(
        long appDateId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = dateDataOfAppDateRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.AppDateId == appDateId);

        var data = await queryable.SingleOrDefaultAsync(
            cancellationToken
        );

        return data;
    }

    public async Task<AppDateAppDateData?> FindByAppDateAsync(
        long appDate,
        CancellationToken cancellationToken = default
    )
    {
        var date = AppDate.GetDateTime(appDate);

        var queryable = dateDataOfAppDateRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.AppDateData)
            .Where(x => x.AppDate!.DateTime == date);

        var data = await queryable.SingleOrDefaultAsync(
            cancellationToken
        );

        return data;
    }
}
