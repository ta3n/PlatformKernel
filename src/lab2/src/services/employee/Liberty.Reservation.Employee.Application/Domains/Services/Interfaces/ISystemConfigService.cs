using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface ISystemConfigService : IBaseService<SystemConfig>
{
    Task UpdateCanOnlinePaymentOfSystemConfigAsync(
        SystemConfig entityToUpdate,
        CancellationToken cancellationToken = default
    );
}
