using AutoMapper.QueryableExtensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.SystemConfig;

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
            .ProjectTo<SystemConfigResponse>(Mapper.ConfigurationProvider);
        var systemConfig = await queryable.FirstOrDefaultAsync(cancellationToken) ?? throw new SystemConfigNotfoundException();
        return (new HeaderDictionary(), systemConfig);
    }
}
