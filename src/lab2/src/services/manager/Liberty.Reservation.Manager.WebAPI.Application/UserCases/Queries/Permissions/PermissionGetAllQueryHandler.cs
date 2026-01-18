using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Permissions;

public class PermissionGetAllQueryHandler(
    IMapper mapper,
    IPermissionService permissionService
) : QuerySingleBaseHandler<PermissionGetAllQuery, IEnumerable<PermissionResponse>>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<PermissionResponse>)> HandleAsync(
        PermissionGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var response = await permissionService.GetAllPermissionFlatAsync(
            cancellationToken
        );

        var header = new HeaderDictionary();

        return (header, response);
    }
}
