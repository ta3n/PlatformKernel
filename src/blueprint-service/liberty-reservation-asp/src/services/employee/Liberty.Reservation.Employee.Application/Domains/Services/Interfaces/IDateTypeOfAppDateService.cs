using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface IDateTypeOfAppDateService : IBaseServiceRelation<AppDateAppDateType>
{
    Task<AppDateAppDateType?> FindByAppDateIdAsync(
        long appDateId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<AppDateAppDateType>> FindAllByAppDateAsync(
        long appDate,
        CancellationToken cancellationToken = default
    );
}
