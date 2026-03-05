using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.SystemConfig;

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

        var systemConfig = await queryable.SingleOrDefaultAsync(
                cancellationToken
            )
            ?? throw new SystemConfigNotfoundException();

        return (new HeaderDictionary(), systemConfig);
    }
}
