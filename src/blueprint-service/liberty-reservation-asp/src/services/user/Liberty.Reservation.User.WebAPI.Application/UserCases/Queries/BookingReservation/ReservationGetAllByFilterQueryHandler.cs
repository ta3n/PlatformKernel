using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.User.Application.Auth;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Specifications;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationGetAllByFilterQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IReservationRepository reservationRepository
) : QueryPageBaseHandler<ReservationGetAllByFilterQuery, ReservationResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<ReservationResponse>)> HandleAsync(
        ReservationGetAllByFilterQuery request,
        CancellationToken cancellationToken
    )
    {
        var userCode = securityContextAccessor.ApplicationUserKey;

        var spec = new ReservationGetAllByFilterQuerySpec(
            request.bookingFilterRequest
        );

        var queryable = reservationRepository
            .GetQueryableWithAsNoTracking(spec)
            .IgnoreQueryFilters()
            .Where(x => x.UserCode == userCode)
            .OrderByDescending(x => x.ReservationDateTime)
            .ProjectTo<ReservationResponse>(Mapper.ConfigurationProvider)
            .AsSingleQuery();

        var page = await queryable.UsePageableAsync(
            request.Pageable,
            cancellationToken: cancellationToken
        );

        var headers = page.GeneratePaginationHttpHeaders();
        var data = page.Content;

        return (headers, data);
    }
}
