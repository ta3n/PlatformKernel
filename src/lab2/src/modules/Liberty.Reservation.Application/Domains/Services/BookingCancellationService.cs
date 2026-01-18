using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingCancellationService(
    ILogger<BookingCancellationService> logger,
    IBookingCancellationRepository bookingCancellationRepository
) : BaseService<Cancellation>(logger, bookingCancellationRepository, new BookingCancellationNotfoundException()),
    IBookingCancellationService
{
    public async Task<BookingCancellationPolicyModel?> GetCancellationPolicyAsync(
        long cancellationId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = bookingCancellationRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == cancellationId)
            .Select(
                x => new BookingCancellationPolicyModel
                {
                    Id = x.Id,
                    CanOnLinePayment = x.CanOnLinePayment,
                    Description = x.Description,
                    Name = x.Name,
                    TableSource = x.TableSource,
                    PaymentLimit = x.PaymentLimit,
                    RuleDetail = x.RuleDetail,
                    CancellationData = x.CancellationCancellationDatas!.Select(
                        a => new BookingCancellationDataPolicyModel
                        {
                            DayEnd = a.CancellationData!.DayEnd,
                            DayStart = a.CancellationData!.DayStart,
                            Rate = a.CancellationData!.Rate
                        }
                    )
                }
            );

        return await queryable.SingleOrDefaultAsync(cancellationToken);
    }
}
