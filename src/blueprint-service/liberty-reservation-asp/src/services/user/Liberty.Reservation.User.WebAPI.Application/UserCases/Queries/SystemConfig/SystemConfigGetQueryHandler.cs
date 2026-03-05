using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.Application.Exceptions;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.SystemConfig;

public class SystemConfigGetQueryHandler(
    IMapper mapper,
    ISystemConfigRepository systemConfigRepository
) : QuerySingleBaseHandler<SystemConfigGetQuery, SystemConfigResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, SystemConfigResponse)> HandleAsync(
        SystemConfigGetQuery request,
        CancellationToken cancellationToken
    )
    {
        var queryable = systemConfigRepository
            .GetQueryableWithAsNoTracking()
            .Select(
                x => new SystemConfigResponse(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.CanOnlinePayment
                )
            );

        var systemConfig = await queryable.FirstOrDefaultAsync(cancellationToken)
            ?? throw new SystemConfigNotfoundException();

        return (new HeaderDictionary(), systemConfig);
    }
}
