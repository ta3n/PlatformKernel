using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

public interface IAlertMessageService : IBaseService<AlertMessage>
{
    Task<AlertMessage> GetFirstAlertMessageAsync(
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistingAlertMessageAsync(
        long id,
        CancellationToken cancellationToken = default
    );
}
