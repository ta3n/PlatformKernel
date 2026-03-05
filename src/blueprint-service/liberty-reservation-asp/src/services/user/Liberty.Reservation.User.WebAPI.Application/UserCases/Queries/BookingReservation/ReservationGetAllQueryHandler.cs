using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.User.Application.Auth;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationGetAllQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IReservationRepository reservationRepository
) : QueryPageBaseHandler<ReservationGetAllQuery, ReservationResponse>(mapper)
{
    private readonly ReservationStatus[] _allowedReservationStates =
    [
        ReservationStatus.Confirmed,
        ReservationStatus.Reserved,
        ReservationStatus.UserCanceled,
        ReservationStatus.Modified,
        ReservationStatus.ManagerCanceled
    ];

    protected override async Task<(IHeaderDictionary, IEnumerable<ReservationResponse>)> HandleAsync(
        ReservationGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var userCode = securityContextAccessor.ApplicationUserKey;

        var queryable = reservationRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.UserCode == userCode)
            .Where(x => _allowedReservationStates.Contains(x.ReservationState))
            .OrderByDescending(x => x.ReservationDateTime)
            .ProjectTo<ReservationResponse>(Mapper.ConfigurationProvider);

        var page = await queryable.UsePageableAsync(
            request.Pageable,
            cancellationToken: cancellationToken
        );

        var headers = page.GeneratePaginationHttpHeaders();
        var data = page.Content;

        return (headers, data);
    }
}
