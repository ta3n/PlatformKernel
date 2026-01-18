using AutoMapper.QueryableExtensions;
using Liberty.Cache.Services;
using Liberty.GmoPaymentGateway.Services;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationGetAllGmoInfoQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    IGmoPaymentResultRequestRepository gmoPaymentResultRequestRepository,
    IGmoErrorCodeService gmoErrorCodeService
) : QueryPageBaseHandler<ReservationGetAllGmoInfoQuery, GmoPaymentResultResponse>(mapper, cacheService)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<GmoPaymentResultResponse>)> HandleAsync(
        ReservationGetAllGmoInfoQuery query,
        CancellationToken cancellationToken
    )
    {
        var queryable = gmoPaymentResultRequestRepository
            .GetQueryableWithAsNoTracking()
            .ProjectTo<GmoPaymentResultResponse>(Mapper.ConfigurationProvider);

        var page = await queryable.UsePageableAsync(
            query.Pageable,
            false,
            cancellationToken
        );

        var headers = page.GeneratePaginationHttpHeaders();
        var data = page.Content;

        var errorLookup = gmoErrorCodeService
            .GetAll()
            .ToDictionary(
                e => $"{e.Code}_{e.DetailCode}",
                e => e.Message
            );

        var updatedData = data.Select(
            item =>
            {
                if (!string.IsNullOrEmpty(item.ErrInfo) && !string.IsNullOrEmpty(item.ErrCode))
                {
                    var key = $"{item.ErrCode}_{item.ErrInfo}";
                    if (errorLookup.TryGetValue(key, out var msg))
                    {
                        item.ErrorMesssage = msg;
                    }
                }

                return item;
            }
        );

        return (headers, updatedData);
    }
}
