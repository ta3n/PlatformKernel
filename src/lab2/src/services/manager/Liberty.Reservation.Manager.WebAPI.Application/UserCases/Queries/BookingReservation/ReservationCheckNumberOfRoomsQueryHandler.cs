using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationCheckNumberOfRoomsQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IReservationRepository reservationRepository,
    IRoomGroupAppDateRepository roomGroupAppDateRepository,
    IPlanRepository planRepository
) : QuerySingleBaseHandler<ReservationCheckNumberOfRoomsQuery, bool>(mapper)
{
    protected override async Task<(IHeaderDictionary, bool)> HandleAsync(
        ReservationCheckNumberOfRoomsQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var reservation = await reservationRepository
                .GetQueryableWithAsNoTracking()
                .Where(
                    x => x.Id == request.Id && x.Facility!.Id == facilityId
                )
                .Select(
                    x => new
                    {
                        x.CheckInDate,
                        RoomGroupId = x.RoomGroup!.Id,
                        x.RoomNumber,
                        x.PlanId
                    }
                )
                .SingleOrDefaultAsync(
                    cancellationToken
                )
            ?? throw new ReservationNotfoundException();

        var isRoomNumberDaySaleLimit = await planRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.IsEnabled)
            .Where(x => x.IsOnLinePayment || x.IsOnSidePayment)
            .Where(x => x.Cancellation!.IsEnabled)
            .Where(x => !x.UseDaySaleLimit || (x.UseDaySaleLimit && x.RoomNumberDaySaleLimit >= reservation.RoomNumber))
            .AnyAsync(x => x.Id == reservation.PlanId, cancellationToken);

        if (!isRoomNumberDaySaleLimit)
        {
            return (new HeaderDictionary(), false);
        }

        var remainNumbers = await roomGroupAppDateRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.AppDateId == reservation.CheckInDate)
            .Where(x => x.RoomGroupId == reservation.RoomGroupId)
            .Where(x => x.RoomGroup!.IsEnabled)
            .Where(x => !x.IsNotSelled)
            .Where(x => x.SellNumber >= reservation.RoomNumber)
            .Select(x => x.RemainNumber)
            .ToListAsync(cancellationToken);

        var isRemainRoomNumber = remainNumbers.Exists(
            x => x >= reservation.RoomNumber
        );

        return (new HeaderDictionary(), isRemainRoomNumber);
    }
}
