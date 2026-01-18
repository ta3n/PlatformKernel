using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationGetMonthsQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IReservationRepository reservationRepository
) : QuerySingleBaseHandler<ReservationGetMonthsQuery, IEnumerable<long>>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<long>)> HandleAsync(
        ReservationGetMonthsQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = reservationRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Facility!.Id == facilityId)
            .Select(x => x.CheckInDate / 100)
            .Distinct();

        var response = await queryable.ToListAsync(cancellationToken);

        var headers = new HeaderDictionary();

        return (headers, response);
    }
}
