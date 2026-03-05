using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class AppDateService(
    ILogger<AppDateService> logger,
    IAppDateRepository appDateRepository
) : BaseService<AppDate>(logger, appDateRepository, new AppDateNotfoundException()), IAppDateService
{
    public async Task<AppDate?> FindByAppDateAsync(
        long appDate,
        CancellationToken cancellationToken = default
    )
    {
        var date = AppDate.GetDateTime(appDate);

        var queryable = appDateRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.DateTime == date);

        var data = await queryable.SingleOrDefaultAsync(
            cancellationToken
        );

        return data;
    }
}
