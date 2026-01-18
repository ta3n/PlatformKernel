using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface IAppDateService : IBaseService<AppDate>
{
    Task<AppDate?> FindByAppDateAsync(
        long appDate,
        CancellationToken cancellationToken = default
    );
}
