using AutoMapper.QueryableExtensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Kakusan;

public class KakusanGetStatusQueryHandler(
    IMapper mapper,
    IRoomAdjustmentStatusRepository roomAdjustmentRepository
) : QuerySingleBaseHandler<KakusanGetStatusQuery, RoomAdjustmentStatusResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, RoomAdjustmentStatusResponse)> HandleAsync(
        KakusanGetStatusQuery query,
        CancellationToken cancellationToken
    )
    {
        var code = query.Code;

        var queryable = roomAdjustmentRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.AdjustmentResults)
            .Where(x => x.Code == code)
            .ProjectTo<RoomAdjustmentStatusResponse>(Mapper.ConfigurationProvider)
            .AsSplitQuery();

        var roomAdjustment = await queryable.SingleOrDefaultAsync(cancellationToken)
            ?? throw new RoomAdjustmentStatusNotfoundException();

        return (new HeaderDictionary(), roomAdjustment);
    }
}
