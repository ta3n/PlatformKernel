using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationGetFilterOptionsQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IReservationRepository reservationRepository
) : QuerySingleBaseHandler<ReservationGetFilterOptionsQuery, ReservationFilterOptionsResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, ReservationFilterOptionsResponse)> HandleAsync(
        ReservationGetFilterOptionsQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var rooms = await reservationRepository
            .GetQueryableWithAsNoTracking()
            .IgnoreQueryFilters()
            .Where(x => x.FacilityId == facilityId)
            .Select(
                rc => new ItemOfFilterOption(
                    rc.RoomGroup!.Id,
                    rc.RoomGroup!.Name!.GetValueByCode(DefaultValues.LanguageCode)
                )
            )
            .Distinct()
            .ToListAsync(cancellationToken);

        var sites = await reservationRepository
            .GetQueryableWithAsNoTracking()
            .IgnoreQueryFilters()
            .Where(x => x.FacilityId == facilityId)
            .Select(
                x => new ItemOfFilterOption(
                    x.Site!.Id,
                    x.Site!.Name!.GetValueByCode(DefaultValues.LanguageCode)
                )
            )
            .Distinct()
            .ToListAsync(cancellationToken);

        var response = new ReservationFilterOptionsResponse(
            [.. ReservationStatusFilter.AllowedStatuses],
            [.. Enum.GetValues<PaymentTypes>().Where(x => x != PaymentTypes.Unknown)],
            rooms,
            sites
        );

        return (new HeaderDictionary(), response);
    }
}
