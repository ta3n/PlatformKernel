using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

public interface IPermissionService : IBaseService<Permission>
{
    Task<IEnumerable<PermissionResponse>> GetAllPermissionAsync(
        PermissionFilterParameter? filterParameter,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<PermissionResponse>> GetAllPermissionFlatAsync(
        CancellationToken cancellationToken = default
    );

    Task<List<Permission>> GetPermissionByKeyAsync(
        string[] key,
        CancellationToken cancellationToken = default
    );

}
