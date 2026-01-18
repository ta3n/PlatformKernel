using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.User.Application.Auth;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationCheckNumberOfNightsQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IReservationRepository reservationRepository,
    IPlanRoomGroupSiteAppDatePriceDataRepository planRoomGroupSiteAppDatePriceDataRepository,
    IPlanRepository planRepository
) : QuerySingleBaseHandler<ReservationCheckNumberOfNightsQuery, bool>(mapper)
{
    protected override async Task<(IHeaderDictionary, bool)> HandleAsync(
        ReservationCheckNumberOfNightsQuery request,
        CancellationToken cancellationToken
    )
    {
        var userCode = securityContextAccessor.ApplicationUserKey;

        var existingReservation = await reservationRepository
                .GetQueryableWithAsNoTracking()
                .Where(
                    x => x.Id == request.Id && x.UserCode == userCode
                )
                .Select(
                    x => new
                    {
                        x.PlanId,
                        x.RoomGroupId,
                        x.SiteId,
                        x.RestNumber,
                        x.CheckInDate,
                        x.CheckOutDate
                    }
                )
                .SingleOrDefaultAsync(
                    cancellationToken
                )
            ?? throw new ReservationNotfoundException();

        var listPlanRoomGroupSiteAppDatePriceData = await planRoomGroupSiteAppDatePriceDataRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.PlanId == existingReservation.PlanId)
            .Where(x => x.Plan!.IsEnabled)
            .Where(x => x.RoomGroupId == existingReservation.RoomGroupId)
            .Where(x => x.RoomGroup!.IsEnabled)
            .Where(x => x.SiteId == existingReservation.SiteId)
            .Where(x => x.Site!.IsEnabled)
            .Where(x => x.DateCalendar >= existingReservation.CheckInDate)
            .Where(x => x.DateCalendar <= existingReservation.CheckOutDate)
            .Where(x => x.PriceData!.Price!.HasValue && x.PriceData!.Price!.Value > 0)
            .ToListAsync(cancellationToken);

        var isNumberOfStayLimit = await planRepository.GetQueryableWithAsNoTracking()
            .Where(x => x.IsEnabled)
            .Where(x => x.IsOnLinePayment || x.IsOnSidePayment)
            .Where(x => x.Cancellation != null && x.Cancellation.IsEnabled)
            .Where(
                x => !x.UseDisplayDate
                    || x.DisplayDateStart == null
                    || x.DisplayDateStart <= existingReservation.CheckInDate
            )
            .Where(
                x => !x.UseDisplayDate
                    || x.DisplayDateEnd == null
                    || x.DisplayDateEnd >= existingReservation.CheckOutDate
            )
            .Where(
                x => !x.UseAcceptDate
                    || x.AcceptDateStart == null
                    || x.AcceptDateStart <= existingReservation.CheckInDate
            )
            .Where(
                x => !x.UseAcceptDate
                    || x.AcceptDateEnd == null
                    || x.AcceptDateEnd >= existingReservation.CheckOutDate
            )
            .Where(x => x.NumberOfStayLimitMax >= existingReservation.RestNumber)
            .Where(x => x.NumberOfStayLimitMin <= existingReservation.RestNumber)
            .AnyAsync(
                x => x.Id == existingReservation.PlanId,
                cancellationToken
            );

        return CheckAvailableChangeNumberOfNights(
            isNumberOfStayLimit,
            existingReservation.RestNumber,
            existingReservation.CheckInDate,
            listPlanRoomGroupSiteAppDatePriceData
        );
    }

    private static (IHeaderDictionary, bool) CheckAvailableChangeNumberOfNights(
        bool isNumberOfStayLimit,
        int existingReservationNights,
        long existingReservationCheckInDate,
        List<PlanRoomGroupSiteAppDatePriceData> listPlanRoomGroupSiteAppDatePriceData
    )
    {
        var header = new HeaderDictionary();
        if (!isNumberOfStayLimit)
        {
            return (header, false);
        }

        var index = 1;
        while (index < existingReservationNights)
        {
            var nextDate = AppDate.GetDateTime(existingReservationCheckInDate).AddDays(index);
            var isPrice = listPlanRoomGroupSiteAppDatePriceData.Exists(
                x => x.DateCalendar == AppDate.GetId(nextDate)
            );
            if (!isPrice)
            {
                return (header, false);
            }

            index++;
        }

        return (header, true);
    }
}
