using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingSystemConfigService(
    ILogger<BookingSystemConfigService> logger,
    IBookingSystemConfigRepository bookingSystemConfigRepository
) : BaseService<SystemConfig>(logger, bookingSystemConfigRepository, new BookingSystemConfigNotfoundException()),
    IBookingSystemConfigService
{
    public async Task<SystemConfig?> GetSystemConfigAsync(
        CancellationToken cancellationToken = default
    )
    {
        var systemConfig = await bookingSystemConfigRepository
            .GetQueryableWithAsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        return systemConfig;
    }

    public async Task<bool> GetSystemConfigOnlinePaymenAsync(
        CancellationToken cancellationToken = default
    )
    {
        var canOnlinePayment = await bookingSystemConfigRepository
                .GetQueryableWithAsNoTracking()
                .Select(c => c.CanOnlinePayment)
                .SingleOrDefaultAsync(cancellationToken)
            ?? throw new BookingSystemConfigNotfoundException();

        return canOnlinePayment;
    }
}
