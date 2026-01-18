using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Application.Domains.Services;

public class GmoChangeTranReportService(
    ILogger<GmoChangeTranReportService> logger,
    IGmoChangeTranReportRepository gmoChangeTranReportRepository
) : BaseService<GmoChangeTranReport>(logger, gmoChangeTranReportRepository, new GmoChangeTranReportNotfoundException()),
    IGmoChangeTranReportService
{
    public async Task<(bool isExisting, string? orderId)> IsExistAsync(
        long reservationId,
        CancellationToken cancellationToken = default
    )
    {
        var gmoReport = await gmoChangeTranReportRepository
            .GetQueryableWithAsNoTracking()
            .SingleOrDefaultAsync(x => x.ReservationId == reservationId, cancellationToken);
        return gmoReport is not null ? (true, gmoReport.OrderId) : (false, null);
    }
}
