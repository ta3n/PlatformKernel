using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

public interface IBookingCancellationService : IBaseService<Cancellation>
{
    Task<BookingCancellationPolicyModel?> GetCancellationPolicyAsync(
        long cancellationId,
        CancellationToken cancellationToken = default
    );
}
