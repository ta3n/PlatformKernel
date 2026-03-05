using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingDetailService(
    IDbContextFactory<DbContext> dbContextFactory,
    IBookingSystemConfigRepository systemConfigRepository,
    IBookingFacilityRepository facilityRepository
) : IBookingDetailService
{
    public async Task<bool> GetSystemCanOnlinePaymentAsync(
        CancellationToken cancellationToken
    )
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var queryable = systemConfigRepository
            .GetQueryableWithAsNoTracking(dbContext)
            .Where(x => !x.IsDeleted)
            .Select(x => x.CanOnlinePayment);

        var facilityCanOnlinePayment = await queryable.FirstOrDefaultAsync(cancellationToken) ?? false;

        return facilityCanOnlinePayment;
    }

    public async Task<FacilityStateModel> GetFacilitySateAsync(
        long facilityId,
        CancellationToken cancellationToken
    )
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var queryable = facilityRepository
            .GetQueryableWithAsNoTracking(dbContext)
            .Where(x => x.Id == facilityId && x.IsEnabled)
            .Select(
                x => new FacilityStateModel(
                    x.Id,
                    x.IsOnLinePayment,
                    x.CanOnLinePayment,
                    x.IsOnSidePayment,
                    x.UseDailyPerson,
                    x.Meta!.UseSpaTax
                )
            );

        var facilityState = await queryable.FirstOrDefaultAsync(cancellationToken)
            ?? throw new BookingFacilityNotfoundException();

        return facilityState;
    }
}
