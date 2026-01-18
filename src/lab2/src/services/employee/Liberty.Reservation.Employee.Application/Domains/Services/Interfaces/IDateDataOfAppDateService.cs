using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface IDateDataOfAppDateService : IBaseServiceRelation<AppDateAppDateData>
{
    Task<AppDateAppDateData?> FindByAppDateIdAsync(
        long appDateId,
        CancellationToken cancellationToken = default
    );

    Task<AppDateAppDateData?> FindByAppDateAsync(
        long appDate,
        CancellationToken cancellationToken = default
    );
}
