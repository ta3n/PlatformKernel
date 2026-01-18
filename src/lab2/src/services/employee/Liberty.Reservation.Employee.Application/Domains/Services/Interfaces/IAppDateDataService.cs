using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface IAppDateDataService : IBaseService<AppDateData>
{
    Task<AppDateData> DeleteByDateIdAsync(
        long appDateId,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<AppDateData?> FindByDateIdAsync(
        long appDateId,
        CancellationToken cancellationToken = default
    );
}
